using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    [DisallowMultipleComponent]
    public abstract class UIWindow : MonoBehaviour, IComparable<UIWindow>
    {
        [SerializeField]
        [UIWindowLayer]
        private int layer;
        [SerializeField]
        private int depth;
        [SerializeField]
        private GameObject defaultSelectedGo;
        [SerializeField]
        private bool firstSelectDefaultGo;
        private GameObject lastSelectedGo;

        private string windowName;
        private bool isShowing;

        public int Layer
        {
            get { return layer; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public string WindowName
        {
            get { return windowName; }
        }

        public bool CanFocus
        {
            get { return defaultSelectedGo != null; }
        }

        public void Hide()
        {
            UIManager.HideWindow(windowName);
        }

        public void Close()
        {
            UIManager.CloseWindow(windowName);
        }

        internal void InitData<T>(string windowName, T data) where T : IGenericData
        {
            this.windowName = windowName;
            OnInitData(data);
        }

        internal void CreateWindow()
        {
            OnCreateWindow();
        }

        internal void CloseWindow()
        {
            OnCloseWindow();
        }

        internal void ShowWindow()
        {
            OnShowWindow();
        }

        internal void HideWindow()
        {
            OnHideWindow();
        }

        internal void Focus()
        {
            if (EventSystem.current == null)
            {
                return;
            }

            GameObject selectedGameObject = defaultSelectedGo;
            if (lastSelectedGo != null && !firstSelectDefaultGo)
            {
                selectedGameObject = lastSelectedGo;
            }

            if (selectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(selectedGameObject);
            }
        }

        internal void UnFocus()
        {
            lastSelectedGo = EventSystem.current.currentSelectedGameObject;
        }

        protected virtual void OnInitData<T>(T data) where T : IGenericData
        {
        }

        protected virtual void OnCreateWindow()
        {
        }

        protected virtual void OnCloseWindow()
        {
        }

        protected virtual void OnShowWindow()
        {
        }

        protected virtual void OnHideWindow()
        {
        }

        public int CompareTo(UIWindow other)
        {
            if (layer != other.layer)
            {
                return layer.CompareTo(other.layer);
            }

            return depth.CompareTo(other.depth);
        }
    }
}
