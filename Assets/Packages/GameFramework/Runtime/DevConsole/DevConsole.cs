using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevConsole : MonoBehaviour
    {
        [SerializeField]
        private GameObject console;
        [SerializeField]
        private GameObject fps;
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Transform tableContent;
        [SerializeField]
        private Transform viewContent;

#if ENABLE_CONSOLE
        private float lastTimeScale;
        private CursorLockMode lastLockMode;
        private bool lastVisible;
        private Dictionary<string, DevGUIViewItem> viewItems;
        private static DevConsole instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainReset()
        {
            instance = null;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialization()
        {
            instance = FindFirstObjectByType<DevConsole>();
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<DevConsole>("DevConsole"));
                instance.name = "DevConsole";
            }

            instance.console.SetActive(false);
            DontDestroyOnLoad(instance.gameObject);
        }

        public static IDevGUISection AddSection(string name)
        {
            if (instance == null)
            {
                Initialization();
            }

            return instance.AddSectionInternal(name);
        }

        public static void RemoveSection(string name)
        {
            if (instance == null)
            {
                return;
            }

            instance.RemoveSectionInternal(name);
        }

        private void Awake()
        {
            InitEventSystem();
            closeBtn.onClick.AddListener(OpenConsole);
            viewItems = new Dictionary<string, DevGUIViewItem>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(GameSetting.Instance.OpenConsole))
            {
                OpenConsole();
            }

            if (Input.touchCount == 5 && !console.activeSelf)
            {
                OpenConsole();
            }
        }

        private void OpenConsole()
        {
            bool isEnable = !console.activeSelf;
            console.SetActive(isEnable);
            fps.SetActive(!isEnable);
            if (isEnable)
            {
                lastLockMode = Cursor.lockState;
                lastVisible = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = lastLockMode;
                Cursor.visible = lastVisible;
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

        private IDevGUISection AddSectionInternal(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name cannot be empty.");
                return null;
            }

            if (viewItems.TryGetValue(name, out DevGUIViewItem item))
            {
                return item.view;
            }

            DevGUITable table = DevResources.Instantiate<DevGUITable>("DevGUITable", tableContent);
            DevGUIView view = DevResources.Instantiate<DevGUIView>("DevGUIView", viewContent);
            table.Init(name, view.gameObject);
            item = new DevGUIViewItem()
            {
                table = table,
                view = view
            };

            viewItems.Add(name, item);
            return view;
        }

        private void RemoveSectionInternal(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            if (viewItems.TryGetValue(name, out DevGUIViewItem item))
            {
                Destroy(item.table.gameObject);
                Destroy(item.view.gameObject);
                viewItems.Remove(name);
            }
        }

        private struct DevGUIViewItem
        {
            public DevGUITable table;
            public DevGUIView view;
        }
#endif
    }
}
