using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIHotkeyWithDisplace : MonoBehaviour
    {
        [SerializeField]
        private string hotkeyId;
        [SerializeField]
        private Image hotkeyImage;
        [SerializeField]
        private TextMeshProUGUI hotkeyText;
        [SerializeField]
        private float triggerRate = 0.1f;
        [SerializeField]
        private InputAction hotkey;
        [SerializeField]
        private UnityEvent onPressed;
        private GameObject imageParent;
        private GameObject textParent;
        private bool isLoadImage;
        private float time;

        private void Awake()
        {
            imageParent = hotkeyImage.transform.parent.gameObject;
            textParent = hotkeyText.transform.parent.gameObject;
            imageParent.SetActiveSafe(false);
            textParent.SetActiveSafe(false);
        }

        private void OnEnable()
        {
            hotkey.Enable();
            OnControllerChanged();
            UIManager.OnControllerChanged += OnControllerChanged;
        }

        private void OnDisable()
        {
            hotkey.Disable();
            UIManager.OnControllerChanged -= OnControllerChanged;
        }

        private void Update()
        {
            if (Time.unscaledTime - time >= triggerRate)
            {
                if (hotkey.triggered)
                {
                    onPressed.Invoke();
                    time = Time.unscaledTime;
                }
            }
        }

        private void OnControllerChanged()
        {
            UIControllerItem item = UIManager.GetControllerItem(hotkeyId);
            if (item != null)
            {
                if (item.itemIcon.RuntimeKeyIsValid())
                {
                    isLoadImage = true;
                    item.LoadSprite().Completed += handle =>
                    {
                        if (isLoadImage)
                        {
                            hotkeyImage.sprite = handle.Result;
                            imageParent.SetActiveSafe(true);
                            textParent.SetActiveSafe(false);
                        }
                    };
                }
                else
                {
                    isLoadImage = false;
                    hotkeyText.text = item.itemText;
                    imageParent.SetActiveSafe(false);
                    textParent.SetActiveSafe(true);
                }
            }
        }
    }
}
