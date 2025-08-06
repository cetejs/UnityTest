using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string name = typeof(T).Name;
                    instance = Resources.Load<T>(name);
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<T>();
                        string path = $"Assets/Resources/{name}.asset";
                        FileUtility.EnsureDirectory(path);
                        AssetDatabase.CreateAsset(instance, path);
                        AssetDatabase.SaveAssets();
                    }
#endif
                }

                return instance;
            }
        }

        public static void Dispose()
        {
            if (instance != null)
            {
                Resources.UnloadAsset(instance);
                instance = null;
            }
        }
    }
}
