using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField]
        private string languageKey;
        private TextMeshProUGUI text;

        public string LanguageKey
        {
            get { return languageKey; }
            set
            {
                if (languageKey != value)
                {
                    languageKey = value;
                    string label = Localization.GetLanguage(languageKey);
                    if (!string.IsNullOrEmpty(label))
                    {
                        SetLanguage(label);
                    }
                    else
                    {
                        SetLanguage(languageKey);
                    }
                }
            }
        }

        public TextMeshProUGUI Text
        {
            get
            {
                if (text == null)
                {
                    text = GetComponent<TextMeshProUGUI>();
                }

                return text;
            }
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(languageKey))
            {
                Debug.LogWarning($"Localization text language key is invalid, {name}", gameObject);
                return;
            }

            SetLanguage(Localization.GetLanguage(languageKey));
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            Localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            SetLanguage(Localization.GetLanguage(languageKey));
        }

        public void SetLanguage(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Text.text = value;
                Text.Rebuild(CanvasUpdate.PreRender);
            }
        }
    }
}
