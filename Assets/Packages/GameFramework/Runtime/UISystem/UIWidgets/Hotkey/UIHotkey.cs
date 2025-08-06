using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameFramework
{
    public class UIHotkey : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private string hotkeyId;
        [SerializeField]
        private string hotkeyLabel;
        [SerializeField]
        private bool activeOnSelected = true;
        [SerializeField]
        private float triggerRate = 0.1f;
        [SerializeField]
        private InputAction hotkey;
        [SerializeField]
        private UnityEvent onPressed;
        private float time;

        private void OnEnable()
        {
            if (activeOnSelected)
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    EnableHotkey();
                }
            }
            else
            {
                EnableHotkey();
            }
        }

        private void OnDisable()
        {
            DisableHotkey();
        }

        private void Update()
        {
            if (!hotkey.enabled)
            {
                return;
            }

            if (Time.unscaledTime - time >= triggerRate)
            {
                if (hotkey.triggered)
                {
                    onPressed.Invoke();
                    time = Time.unscaledTime;
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (activeOnSelected)
            {
                EnableHotkey();
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (activeOnSelected)
            {
                DisableHotkey();
            }
        }

        private void EnableHotkey()
        {
            hotkey.Enable();
            UIManager.EventPool?.Send(UIEventKey.RegisterHotkeyEvent, new GenericData<string, string>()
            {
                Item1 = hotkeyId,
                Item2 = hotkeyLabel
            });
        }

        private void DisableHotkey()
        {
            hotkey.Disable();
            UIManager.EventPool?.Send(UIEventKey.UnregisterHotkeyEvent, new GenericData<string, string>()
            {
                Item1 = hotkeyId,
                Item2 = hotkeyLabel
            });
        }
    }
}
