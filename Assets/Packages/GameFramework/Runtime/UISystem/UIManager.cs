using System;
using UnityEngine.EventSystems;

namespace GameFramework
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        private UIWindowManager windowManager = new UIWindowManager();
        private UIControllerManager controllerManager = new UIControllerManager();
        private EventPool<int> eventPool = new EventPool<int>();

        public static bool SendNavigation
        {
            get { return Instance != null && Instance.controllerManager.sendNavigation; }
            set
            {
                if (Instance != null)
                {
                    Instance.controllerManager.sendNavigation = value;
                }

                if (EventSystem.current != null)
                {
                    EventSystem.current.sendNavigationEvents = value;
                }
            }
        }

        public static bool SendMouseNavigation
        {
            get { return Instance != null && Instance.controllerManager.mouseForceNavigation; }
        }

        public static EventPool<int> EventPool
        {
            get { return Instance?.eventPool; }
        }

        public static event Action OnControllerChanged
        {
            add
            {
                if (Instance != null)
                {
                    Instance.controllerManager.onControllerChanged += value;
                }
            }
            remove
            {
                if (Instance != null)
                {
                    Instance.controllerManager.onControllerChanged -= value;
                }
            }
        }

        private void Awake()
        {
            windowManager.Init(transform);
            windowManager.Awake();
            controllerManager.Awake();
        }

        private void Start()
        {
            controllerManager.Start();
        }

        private void Update()
        {
            controllerManager.Update();
            eventPool.Update();
        }

        private void LateUpdate()
        {
            controllerManager.LateUpdate();
        }

        public static void ShowWindow(string windowName, IGenericData data = null)
        {
            if (Instance != null)
            {
                Instance.windowManager.ShowWindowInternal(windowName, data);
            }
        }

        public static void ShowWindowAsync(string windowName, Action callback)
        {
            ShowWindowAsync(windowName, null, callback);
        }

        public static void ShowWindowAsync(string windowName, IGenericData data = null, Action callback = null)
        {
            if (Instance != null)
            {
                Instance.windowManager.ShowWindowAsyncInternal(windowName, data, callback);
            }
        }

        public static void HideWindow(string windowName)
        {
            if (Instance != null)
            {
                Instance.windowManager.HideWindowInternal(windowName);
            }
        }

        public static void CloseWindow(string windowName)
        {
            if (Instance != null)
            {
                Instance.windowManager.CloseWindowInternal(windowName);
            }
        }

        public static void CloseAllWindow()
        {
            if (Instance != null)
            {
                Instance.windowManager.CloseAllWindowInternal();
            }
        }

        public static void CloseAllWindowExcluding(params string[] excluding)
        {
            if (Instance != null)
            {
                Instance.windowManager.CloseAllWindowExcludingInternal(excluding);
            }
        }

        public static bool HasWindow(string windowName)
        {
            if (Instance != null)
            {
                Instance.windowManager.HasWindowInternal(windowName);
            }

            return false;
        }

        public static T GetWindow<T>(string windowName) where T : UIWindow
        {
            if (Instance != null)
            {
                Instance.windowManager.GetWindowInternal<T>(windowName);
            }

            return null;
        }

        public static UIControllerItem GetControllerItem(string itemName)
        {
            if (Instance != null)
            {
                return Instance.controllerManager.GetControllerItem(itemName);
            }

            return null;
        }
    }
}
