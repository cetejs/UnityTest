using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework
{
    public partial class Localization : Singleton<Localization>
    {
        private Dictionary<string, int> languageKeyMap = new Dictionary<string, int>();
        private List<string> languages = new List<string>();
        private string languageType;

        private Action onLanguageChanged;

        private string GetLanguageInternal(string key)
        {
            if (string.IsNullOrEmpty(languageType))
            {
                return string.Empty;
            }

            if (languageKeyMap.TryGetValue(key, out int index))
            {
                if (index >= 0 && index <= languages.Count)
                {
                    return languages[index];
                }
            }

            Debug.LogError($"Language not found {key}");
            return string.Empty;
        }

        private void ChangeLanguageInternal(string type)
        {
            if (languageType == type)
            {
                return;
            }

            if (string.IsNullOrEmpty(languageType))
            {
                LoadText("LanguageKey.bytes", ReadLanguageKey);
            }

            languageType = type;
            LoadText(string.Concat(type, "/Language.bytes"), ReadLanguage);
            onLanguageChanged?.Invoke();
        }

        private void ChangeLanguageAsyncInternal(string type, Action callback)
        {
            if (languageType == type)
            {
                return;
            }

            if (string.IsNullOrEmpty(languageType))
            {
                LoadText("LanguageKey.bytes", ReadLanguageKey);
            }

            languageType = type;
            LoadTextAsync(string.Concat(languageType, "/Language.bytes"), bytes =>
            {
                ReadLanguage(bytes);
                onLanguageChanged?.Invoke();
                callback?.Invoke();
            });
        }

        private void UnloadLanguageInternal()
        {
            languageType = null;
            languageKeyMap.Clear();
            languages.Clear();
        }

        private void LoadText(string path, Action<byte[]> callback)
        {
            string loadPath = PathUtility.Combine(DataTableSetting.Instance.LoadLocalizationPath, path);
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(loadPath);
            handle.WaitForCompletion();
            TextAsset asset = handle.Result;
            if (asset == null)
            {
                Debug.LogError($"Load localization {loadPath} is fail");
                return;
            }

            DataTableSetting setting = DataTableSetting.Instance;
            if (setting.UseCrypto)
            {
                callback?.Invoke(CryptoUtility.DecryptBytesFromBytes(asset.bytes, setting.Password));
            }
            else
            {
                callback?.Invoke(asset.bytes);
            }

            Addressables.Release(handle);
        }

        private void LoadTextAsync(string path, Action<byte[]> callback)
        {
            string loadPath = PathUtility.Combine(DataTableSetting.Instance.LoadLocalizationPath, path);
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(loadPath);
            handle.Completed += _ =>
            {
                TextAsset asset = handle.Result;
                if (asset == null)
                {
                    Debug.LogError($"Load localization {loadPath} is fail");
                    return;
                }

                DataTableSetting setting = DataTableSetting.Instance;
                if (setting.UseCrypto)
                {
                    callback?.Invoke(CryptoUtility.DecryptBytesFromBytes(asset.bytes, setting.Password));
                }
                else
                {
                    callback?.Invoke(asset.bytes);
                }

                Addressables.Release(handle);
            };
        }

        private void ReadLanguageKey(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    languageKeyMap.Clear();
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        languageKeyMap.Add(key, i);
                    }
                }
            }
        }

        private void ReadLanguage(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    languages.Clear();
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string language = reader.ReadString();
                        languages.Add(language);
                    }
                }
            }
        }
    }
}
