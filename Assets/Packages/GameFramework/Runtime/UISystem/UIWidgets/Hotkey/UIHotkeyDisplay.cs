using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIHotkeyDisplay : PoolObject
    {
        [SerializeField]
        private Image hotkeyImage;
        [SerializeField]
        private TextMeshProUGUI hotkeyText;
        [SerializeField]
        private TextMeshProUGUI hotkeyLabelText;
        private GameObject imageParent;
        private GameObject textParent;
        private RectTransform rectTransform;
        private bool isLoadImage;

        public string HotkeyId { get; private set; }
        public string HotkeyLabel { get; private set; }

        public int RegisterCount { get; set; }
        public bool ReadyToClear => RegisterCount <= 0;

        private void Awake()
        {
            imageParent = hotkeyImage.transform.parent.gameObject;
            textParent = hotkeyText.transform.parent.gameObject;
            rectTransform = transform as RectTransform;
        }

        public void SetData(string hotkeyId, string hotkeyLabel)
        {
            HotkeyId = hotkeyId;
            HotkeyLabel = hotkeyLabel;
            hotkeyLabelText.text = Localization.GetLanguage(hotkeyLabel);
            OnControllerChanged();
        }

        protected override void OnWakeUp()
        {
            UIManager.OnControllerChanged += OnControllerChanged;
            Localization.OnLanguageChanged += OnLanguageChanged;
            imageParent.SetActiveSafe(false);
            textParent.SetActiveSafe(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        protected override void OnSleep()
        {
            UIManager.OnControllerChanged -= OnControllerChanged;
            Localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnControllerChanged()
        {
            UIControllerItem item = UIManager.GetControllerItem(HotkeyId);
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
                            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                        }
                    };
                }
                else
                {
                    isLoadImage = false;
                    hotkeyText.text = item.itemText;
                    imageParent.SetActiveSafe(false);
                    textParent.SetActiveSafe(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                }
            }
        }

        private void OnLanguageChanged()
        {
            hotkeyLabelText.text = Localization.GetLanguage(HotkeyLabel);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
