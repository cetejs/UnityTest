using System;

namespace GameFramework
{
    public partial class Localization
    {
        public static string LanguageType
        {
            get { return Instance?.languageType; }
        }

        public static event Action OnLanguageChanged
        {
            add
            {
                if (Instance != null) Instance.onLanguageChanged += value;
            }

            remove
            {
                if (Instance != null) Instance.onLanguageChanged -= value;
            }
        }

        public static string GetLanguage(string key)
        {
            return Instance?.GetLanguageInternal(key);
        }

        public static void ChangeLanguage(string type)
        {
            Instance?.ChangeLanguageInternal(type);
        }

        public static void ChangeLanguageAsync(string type, Action callback = null)
        {
            Instance?.ChangeLanguageAsyncInternal(type, callback);
        }

        public static void UnloadLanguage()
        {
            Instance?.UnloadLanguageInternal();
        }
    }
}
