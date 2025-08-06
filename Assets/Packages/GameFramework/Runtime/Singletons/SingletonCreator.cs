using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal static class SingletonCreator
    {
        private static bool qutiting;
        private static Dictionary<Type, object> singletons = new Dictionary<Type, object>();

        static SingletonCreator()
        {
            Application.quitting += () => { qutiting = true; };
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainReset()
        {
            qutiting = false;
            singletons.Clear();
        }

        public static T GetSingleton<T>() where T : class, new()
        {
            Type type = typeof(T);
            if (singletons.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            T instance = new T();
            singletons.Add(type, instance);
            return instance;
        }

        public static T GetMonoSingleton<T>(bool dontDestroyOnLoad) where T : MonoBehaviour
        {
            if (qutiting)
            {
                return null;
            }

            Type type = typeof(T);
            if (singletons.TryGetValue(type, out object obj) && obj != null)
            {
                return obj as T;
            }

            T instance = Object.FindFirstObjectByType<T>();
            if (instance == null)
            {
                instance = new GameObject(type.Name).AddComponent<T>();
            }

            if (dontDestroyOnLoad)
            {
                Object.DontDestroyOnLoad(instance.gameObject);
            }

            singletons[type] = instance;
            return instance;
        }
    }
}
