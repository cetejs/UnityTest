using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject defaultSelectedGo;
        [SerializeField]
        private GameObject cancelSelectGo;
        [SerializeField]
        private UnityEvent onSelectedCancel;

        protected virtual void Update()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                OnFocus();
            }
        }

        public virtual void OnFocus()
        {
            if (EventSystem.current == null)
            {
                return;
            }

            GameObject selectedGameObject = defaultSelectedGo;

            if (defaultSelectedGo == null)
            {
                Selectable firstSelectable = GetComponentInChildren<Selectable>();
                if (firstSelectable != null)
                {
                    selectedGameObject = firstSelectable.gameObject;
                }
            }

            if (selectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(selectedGameObject);
            }
        }

        public virtual void OnCancel()
        {
            if (cancelSelectGo != null)
            {
                EventSystem.current.SetSelectedGameObject(cancelSelectGo);
            }
            else
            {
                onSelectedCancel.Invoke();
            }
        }
    }
}
