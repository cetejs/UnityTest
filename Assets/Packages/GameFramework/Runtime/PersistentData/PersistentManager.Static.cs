using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class PersistentManager
    {
        public static event Action<string> OnStorageLoading
        {
            add
            {
                if (Instance != null) Instance.onStorageLoading += value;
            }
            remove
            {
                if (Instance != null) Instance.onStorageLoading -= value;
            }
        }

        public static event Action<string> OnStorageSaving
        {
            add
            {
                if (Instance != null) Instance.onStorageSaving += value;
            }
            remove
            {
                if (Instance != null) Instance.onStorageSaving -= value;
            }
        }

        public static IPersistentStorage GetStorage(string storageName)
        {
            return Instance?.GetStorageInternal(storageName);
        }

        public static IPersistentStorage Load(string storageName)
        {
            return Instance?.LoadInternal(storageName);
        }

        public static StorageAsyncOperation LoadAsync(string storageName)
        {
            return Instance?.LoadAsyncInternal(storageName);
        }

        public static void Unload(string storageName)
        {
            Instance?.UnloadInternal(storageName);
        }

        public static void Save(string storageName)
        {
            Instance?.SaveInternal(storageName);
        }

        public static StorageAsyncOperation SaveAsync(string storageName)
        {
            return Instance?.SaveAsyncInternal(storageName);
        }

        public static T GetData<T>(string storageName, string key, T defaultValue = default)
        {
            return Instance == null ? defaultValue : Instance.GetDataInternal(storageName, key, defaultValue);
        }

        public static void SetData<T>(string storageName, string key, T value)
        {
            Instance?.GetStorageInternal(storageName).SetData(key, value);
        }
        
        public static void SaveData<T>(string storageName, string key, T value)
        {
            Instance?.GetStorageInternal(storageName).SetData(key, value);
            Instance?.GetStorageInternal(storageName).Save();
        }

        public static string[] GetAllKeys(string storageName)
        {
            return Instance?.GetStorageInternal(storageName).GetAllKeys();
        }

        public static void GetAllKeys(string storageName, List<string> result)
        {
            Instance?.GetStorageInternal(storageName).GetAllKeys(result);
        }

        public static bool HasKey(string storageName, string key)
        {
            return Instance != null && Instance.HasKeyInternal(storageName, key);
        }

        public static void DeleteKey(string storageName, string key)
        {
            Instance?.DeleteKeyInternal(storageName, key);
        }

        public static void DeleteNode(string storageName, string key)
        {
            Instance?.DeleteNodeInternal(storageName, key);
        }

        public static void Delete(string storageName)
        {
            Instance?.DeleteInternal(storageName);
        }
    }
}
