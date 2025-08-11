using System.Collections.Generic;
using GameFramework;
using TMPro;
using UnityEngine;

namespace Tests
{
    public class UIDisplaySetting : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown fullScreenDropdown;
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        [SerializeField]
        private TMP_Dropdown languageDropdown;

        private List<Resolution> resolutions = new List<Resolution>();

        private static List<Language> Languages = new List<Language>()
        {
            new Language("CN", "简体中文"),
            new Language("EN", "Engine"),
            new Language("JP", "日本语"),
        };

        private void Awake()
        {
#if UNITY_STANDALONE_WIN || Unity_STANDALONE_OSX
            InitFullScreenDropdown();
            InitResolutionDropdown();
#else
            fullScreenDropdown.gameObject.SetActive(false);
            resolutionDropdown.gameObject.SetActive(false);
#endif

            InitLanguageDropdown();
        }

        private void OnEnable()
        {
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            Localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
#if UNITY_STANDALON || UNITY_EDITOR
            InitFullScreenDropdown();
#endif
        }

        private void InitFullScreenDropdown()
        {
            int index = -1;
            List<string> options = new List<string>(3);
#if Unity_STANDALONE_OSX
            int invalidIndex = 0;
#else
            int invalidIndex = 2;
#endif
            for (int i = 0; i < 4; i++)
            {
                if (i == invalidIndex)
                {
                    continue;
                }

                if (index == -1 && (FullScreenMode) i == Screen.fullScreenMode)
                {
                    index = options.Count;
                }

                options.Add(Localization.GetLanguage(((FullScreenMode) i).ToString()));
            }

            fullScreenDropdown.ClearOptions();
            fullScreenDropdown.AddOptions(options);
            fullScreenDropdown.SetValueWithoutNotify(index);
            fullScreenDropdown.onValueChanged.AddListener(OnFullScreenDropdownChanged);
        }

        private void OnFullScreenDropdownChanged(int value)
        {
#if Unity_STANDALONE_OSX
            value += 1;
#else
            if (value == 2)
            {
                value = 3;
            }
#endif

            Screen.fullScreenMode = (FullScreenMode) value;
            PersistentManager.SaveData("GameSettings", "FullScreen", value);
        }

        private void InitResolutionDropdown()
        {
            resolutions.Clear();
            foreach (Resolution resolution in Screen.resolutions)
            {
                if (resolutions.FindIndex(temp => temp.height == resolution.height && temp.width == resolution.width) < 0)
                {
                    resolutions.Add(resolution);
                }
            }

            int index = -1;
            List<string> options = new List<string>(Screen.resolutions.Length);
            for (int i = 0; i < resolutions.Count; i++)
            {
                Resolution resolution = resolutions[i];
                if (index == -1 && resolution.width == Screen.width && resolution.height == Screen.height)
                {
                    index = i;
                }

                options.Add(string.Concat(resolution.width, " x ", resolution.height));
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.SetValueWithoutNotify(index);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
        }

        private void OnResolutionDropdownChanged(int value)
        {
            Resolution resolution = resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            PersistentManager.SetData("GameSettings", "Resolution.x", resolution.width);
            PersistentManager.SetData("GameSettings", "Resolution.y", resolution.height);
            PersistentManager.Save("GameSettings");
        }

        private void InitLanguageDropdown()
        {
            int index = -1;
            List<string> options = new List<string>(Languages.Count);
            for (int i = 0; i < Languages.Count; i++)
            {
                Language language = Languages[i];
                if (index == -1 && language.Key == Localization.LanguageType)
                {
                    index = i;
                }

                options.Add(language.Name);
            }

            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(options);
            languageDropdown.SetValueWithoutNotify(index);
            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
        }

        private void OnLanguageDropdownChanged(int value)
        {
            Language language = Languages[value];
            Localization.ChangeLanguageAsync(language.Key);
            PersistentManager.SaveData("GameSettings", "Localization", language.Key);
        }

        private class Language
        {
            public readonly string Key;
            public readonly string Name;

            public Language(string key, string name)
            {
                Key = key;
                Name = name;
            }
        }
    }
}
