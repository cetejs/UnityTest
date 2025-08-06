using System;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    public class PersistentSetting : ScriptableObjectSingleton<PersistentSetting>
    {
        public string SaveDirectory = "SaveData";
        public string DefaultStorageName = "DefaultStorage";
        public string SaveDataExtension = "dat";
        public StorageMode StorageMode = StorageMode.Json;
        public bool UseCrypto = false;
        [Condition("UseCrypto", true)]
        public string Password = "password";

        private string SaveDataPath
        {
            get
            {
#if UNITY_EDITOR
                return PathUtility.Combine(Application.streamingAssetsPath, SaveDirectory);
#else
                return PathUtility.Combine(Application.persistentDataPath, SaveDirectory);
#endif
            }
        }

        public string GetSavePath(string storageName)
        {
            string path = PathUtility.Combine(SaveDataPath, storageName);
            if (string.IsNullOrEmpty(SaveDataExtension) || storageName.LastIndexOf(".", StringComparison.Ordinal) >= 0)
            {
                return path;
            }

            return string.Concat(path, ".", SaveDataExtension);
        }
    }

    public enum StorageMode
    {
        Json,
        Binary
    }
}
