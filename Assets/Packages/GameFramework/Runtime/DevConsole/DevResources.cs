using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal static class DevResources
    {
        private static Dictionary<string, GameObject> resources = new Dictionary<string, GameObject>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainReset()
        {
            resources.Clear();
        }

        public static T Instantiate<T>(string path, Transform parent) where T : MonoBehaviour
        {
            if (resources.TryGetValue(path, out GameObject go))
            {
                return Object.Instantiate(go, parent).GetComponent<T>();
            }

            T result = Resources.Load<T>(path);
            resources.Add(path, result.gameObject);
            return Object.Instantiate(result, parent);
        }

        public static T InstantiateSelector<T>(Type type, Transform parent) where T : DevGUISelector
        {
            if (typeof(Delegate).IsAssignableFrom(type) || type == typeof(object) || type == typeof(Enum))
            {
                return null;
            }

            if (type == typeof(bool))
            {
                return Instantiate<T>("DevGUIBoolSelector", parent);
            }

            if (type.IsPrimitive && type != typeof(char) || type == typeof(decimal))
            {
                return Instantiate<T>("DevGUIValueSelector", parent);
            }

            if (type.IsSubclassOf(typeof(Enum)))
            {
                return Instantiate<T>("DevGUIEnumSelector", parent);
            }

            return Instantiate<T>("DevGUIJsonSelector", parent);
        }
    }
}
