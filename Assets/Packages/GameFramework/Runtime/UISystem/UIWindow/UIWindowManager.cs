using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal partial class UIWindowManager
    {
        private Transform transform;
        private Transform windowRoot;
        private Dictionary<string, UIWindowReference> windowReferences = new Dictionary<string, UIWindowReference>();
        private Dictionary<string, UIWindow> allWindows = new Dictionary<string, UIWindow>();
        private Dictionary<int, Transform> allLayers = new Dictionary<int, Transform>();
        private Stack<UIWindow> fullScreenWindows = new Stack<UIWindow>();
        private SortedLinkedList<UIWindow> selectableWindows = new SortedLinkedList<UIWindow>();
        private List<string> toRemoveWindows = new List<string>();
        private GameObject lastSelectedGameObject;

        public void Init(Transform transform)
        {
            this.transform = transform;
        }

        public void Awake()
        {
            BuildWindowLayers();
            InitEventSystem();
        }

        private void BuildWindowLayers()
        {
            UISetting setting = UISetting.Instance;
            if (setting.WindowRoot.RuntimeKeyIsValid())
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(setting.WindowRoot, transform);
                handle.WaitForCompletion();
                windowRoot = handle.Result.transform;
#if UNITY_EDITOR
                windowRoot.name = windowRoot.name.Replace("(Clone)", "");
#endif
                for (int i = 0; i < setting.WindowLayers.Length; i++)
                {
                    AddWindowLayer(i, setting.WindowLayers[i]);
                }
            }
            else
            {
                windowRoot = new GameObject("WindowRoot").AddComponent<RectTransform>();
                Canvas canvas = windowRoot.gameObject.AddComponent<Canvas>();
                CanvasScaler scaler = windowRoot.gameObject.AddComponent<CanvasScaler>();
                windowRoot.gameObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                windowRoot.SetParent(transform);
                for (int i = 0; i < setting.WindowLayers.Length; i++)
                {
                    AddWindowLayer(i, setting.WindowLayers[i]);
                }
            }
        }

        private void InitEventSystem()
        {
            if (EventSystem.current == null)
            {
                EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();
                InputSystemUIInputModule inputModule = EventSystem.current.gameObject.GetOrAddComponent<InputSystemUIInputModule>();
                inputModule.UnassignActions();
                inputModule.AssignDefaultActions();
                inputModule.transform.SetParent(transform);
            }
        }

        private void AddWindowLayer(int layer, string layerName)
        {
            if (!windowRoot.Find(layerName))
            {
                RectTransform rectTrs = new GameObject(layerName).AddComponent<RectTransform>();
                rectTrs.SetParent(windowRoot);
                rectTrs.AdjustAnchor(AnchorLeftType.Stretch, AnchorTopType.Stretch);
                rectTrs.ResetLocal();
                allLayers.Add(layer, rectTrs);
            }
        }

        private void SetSiblingIndex(UIWindow window)
        {
            int index = 0;
            foreach (UIWindow tempWindow in allWindows.Values)
            {
                if (tempWindow.Layer != window.Layer)
                {
                    continue;
                }

                if (tempWindow.Depth > window.Depth)
                {
                    continue;
                }

                int tempIndex = tempWindow.transform.GetSiblingIndex();
                index = Mathf.Max(index, tempIndex + 1);
            }

            window.transform.SetSiblingIndex(index);
        }

        public void ShowWindowInternal<T>(string windowName, T data) where T : IGenericData
        {
            if (allWindows.TryGetValue(windowName, out UIWindow window))
            {
                if (window.gameObject.activeSelf)
                {
                    Debug.LogError($"UIWindow {windowName} is already show");
                    return;
                }

                ShowWindowInternal(window, windowName, data);
                return;
            }

            if (windowReferences.TryGetValue(windowName, out UIWindowReference reference))
            {
                if (!reference.IsDone)
                {
                    Debug.LogError($"UIWindow {windowName} is loading");
                    return;
                }
            }
            else
            {
                reference = new UIWindowReference(windowName);
                windowReferences.Add(windowName, reference);
            }

            window = CreateWindowInternal(reference.Get());
            ShowWindowInternal(window, windowName, data);
            allWindows.Add(windowName, window);
        }

        public void ShowWindowAsyncInternal<T>(string windowName, T data, Action callback) where T : IGenericData
        {
            if (allWindows.TryGetValue(windowName, out UIWindow window))
            {
                if (window.isActiveAndEnabled)
                {
                    Debug.LogError($"UIWindow {windowName} is already show");
                    return;
                }

                ShowWindowInternal(window, windowName, data);
                callback?.Invoke();
                return;
            }

            if (windowReferences.TryGetValue(windowName, out UIWindowReference reference))
            {
                if (!reference.IsDone)
                {
                    Debug.LogError($"UIWindow {windowName} is loading");
                    return;
                }
            }
            else
            {
                reference = new UIWindowReference(windowName);
                windowReferences.Add(windowName, reference);
            }

            reference.GetAsync(prefab =>
            {
                window = CreateWindowInternal(prefab);
                ShowWindowInternal(window, windowName, data);
                allWindows.Add(windowName, window);
                callback?.Invoke();
            });
        }

        public void HideWindowInternal(string windowName)
        {
            if (allWindows.TryGetValue(windowName, out UIWindow window) && window.isActiveAndEnabled)
            {
                window.gameObject.SetActive(false);
                window.HideWindow();
                PopFullWindow(window);
                if (window.CanFocus)
                {
                    selectableWindows.Remove(window);
                    SelectWindowInternal();
                }
            }
            else
            {
                Debug.LogError($"UIWindow {windowName} is not exist in show windows");
            }
        }

        public void CloseWindowInternal(string windowName)
        {
            if (allWindows.TryGetValue(windowName, out UIWindow window))
            {
                window.CloseWindow();
                PopFullWindow(window);
                Object.Destroy(window.gameObject);
                allWindows.Remove(windowName);
                if (window.CanFocus)
                {
                    selectableWindows.Remove(window);
                    SelectWindowInternal();
                }
            }
            else
            {
                Debug.LogError($"UIWindow {windowName} is not exist in show windows");
            }
        }

        public void CloseAllWindowInternal()
        {
            foreach (UIWindow window in allWindows.Values)
            {
                window.CloseWindow();
                Object.Destroy(window.gameObject);
            }

            allWindows.Clear();
            selectableWindows.Clear();
            fullScreenWindows.Clear();
        }

        public void CloseAllWindowExcludingInternal(params string[] excluding)
        {
            if (excluding.Length == 0)
            {
                CloseAllWindowInternal();
                return;
            }

            toRemoveWindows.Clear();
            foreach (UIWindow window in allWindows.Values)
            {
                if (excluding.Contains(window.WindowName))
                {
                    continue;
                }

                toRemoveWindows.Add(window.WindowName);
            }

            for (int i = toRemoveWindows.Count - 1; i >= 0; i--)
            {
                CloseWindowInternal(toRemoveWindows[i]);
            }
        }

        public bool HasWindowInternal(string windowName)
        {
            return allWindows.ContainsKey(windowName);
        }

        public T GetWindowInternal<T>(string windowName) where T : UIWindow
        {
            allWindows.TryGetValue(windowName, out UIWindow window);
            return window as T;
        }

        private UIWindow CreateWindowInternal(UIWindow prefab)
        {
            Transform parent = allLayers[prefab.Layer];
            UIWindow window = Object.Instantiate(prefab, parent);
#if UNITY_EDITOR
            window.name = window.name.Replace("(Clone)", "");
#endif
            SetSiblingIndex(window);
            window.CreateWindow();
            return window;
        }

        private void ShowWindowInternal<T>(UIWindow window, string windowName, T data) where T : IGenericData
        {
            PushFullWindow(window);
            window.gameObject.SetActive(true);
            window.InitData(windowName, data);
            window.ShowWindow();
            if (window.CanFocus)
            {
                selectableWindows.Add(window);
                SelectWindowInternal();
            }
        }

        private void PushFullWindow(UIWindow window)
        {
            if (window.Layer > 0)
            {
                return;
            }

            if (fullScreenWindows.Count > 0)
            {
                UIWindow lastUIWindow = fullScreenWindows.Peek();
                lastUIWindow.UnFocus();
                lastUIWindow.gameObject.SetActive(false);
            }

            fullScreenWindows.Push(window);
        }

        private void PopFullWindow(UIWindow window)
        {
            if (window.Layer > 0)
            {
                return;
            }

            window.UnFocus();
            fullScreenWindows.Pop();
            if (fullScreenWindows.Count > 0)
            {
                UIWindow lastUIWindow = fullScreenWindows.Peek();
                lastUIWindow.gameObject.SetActive(true);
            }
        }

        private void SelectWindowInternal()
        {
            if (selectableWindows.Count > 0 && EventSystem.current != null)
            {
                selectableWindows.Last.Value.Focus();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void FixInputSystem()
        {
            typeof(InputSystemUIInputModule).GetField("defaultActions", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        }
    }
}
