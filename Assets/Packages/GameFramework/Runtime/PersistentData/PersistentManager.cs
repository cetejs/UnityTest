using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public partial class PersistentManager : Singleton<PersistentManager>
    {
        private Dictionary<string, IPersistentStorage> storages = new Dictionary<string, IPersistentStorage>();
        private Action<string> onStorageLoading;
        private Action<string> onStorageSaving;

        private IPersistentStorage GetStorageInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is get fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage = LoadInternal(storageName);
            }

            return storage;
        }

        private IPersistentStorage LoadInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is load fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                switch (PersistentSetting.Instance.StorageMode)
                {
                    case StorageMode.Json:
                        storage = new PersistentJsonStorage();
                        break;
                    case StorageMode.Binary:
                        storage = new PersistentBinaryStorage();
                        break;
                    default:
                        storage = new PersistentJsonStorage();
                        break;
                }

                storages.Add(storageName, storage);
            }

            storage.Load(storageName);
            onStorageLoading?.Invoke(storageName);
            return storage;
        }

        private StorageAsyncOperation LoadAsyncInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is load fail, because storage name is invalid");
                return null;
            }

            if (!storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                switch (PersistentSetting.Instance.StorageMode)
                {
                    case StorageMode.Json:
                        storage = new PersistentJsonStorage();
                        break;
                    case StorageMode.Binary:
                        storage = new PersistentBinaryStorage();
                        break;
                    default:
                        storage = new PersistentJsonStorage();
                        break;
                }

                storages.Add(storageName, storage);
            }

            StorageAsyncOperation operation = storage.LoadAsync(storageName);
            operation.OnCompleted += _ => { onStorageLoading?.Invoke(storageName); };
            return operation;
        }

        private void UnloadInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is unload fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage.Unload();
                storages.Remove(storageName);
            }
            else
            {
                Debug.LogError($"Storage is unload fail, because storage {storageName} is not loaded");
            }
        }

        private void SaveInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is save fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                onStorageSaving?.Invoke(storageName);
                storage.Save();
            }
            else
            {
                Debug.LogError($"Storage is save fail, because storage {storageName} is not loaded");
            }
        }

        private StorageAsyncOperation SaveAsyncInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is save fail, because storage name is invalid");
                return null;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                onStorageSaving?.Invoke(storageName);
                return storage.SaveAsync();
            }

            Debug.LogError($"Storage is save fail, because storage {storageName} is not loaded");
            return null;
        }

        private T GetDataInternal<T>(string storageName, string key, T defaultValue = default)
        {
            return GetStorageInternal(storageName).GetData(key, defaultValue);
        }

        private void SetDataInternal<T>(string storageName, string key, T value)
        {
            GetStorageInternal(storageName).SetData(key, value);
        }

        private string[] GetAllKeysInternal(string storageName)
        {
            return GetStorageInternal(storageName).GetAllKeys();
        }

        private void GetAllKeysInternal(string storageName, List<string> results)
        {
            GetStorageInternal(storageName).GetAllKeys(results);
        }

        private bool HasKeyInternal(string storageName, string key)
        {
            return GetStorageInternal(storageName).HasKey(key);
        }

        private void DeleteKeyInternal(string storageName, string key)
        {
            GetStorageInternal(storageName).DeleteKey(key);
        }

        private void DeleteNodeInternal(string storageName, string key)
        {
            GetStorageInternal(storageName).DeleteNode(key);
        }

        private void DeleteInternal(string storageName)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                Debug.LogError("Storage is load fail, because storage name is invalid");
                return;
            }

            if (storages.TryGetValue(storageName, out IPersistentStorage storage))
            {
                storage.Unload();
                storages.Remove(storageName);
            }

            string savePath = PersistentSetting.Instance.GetSavePath(storageName);
            FileUtility.DeleteFile(savePath);
#if UNITY_EDITOR
            FileUtility.DeleteFile(string.Concat(savePath, ".meta"));
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
