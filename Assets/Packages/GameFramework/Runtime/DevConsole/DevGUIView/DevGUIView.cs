using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    internal class DevGUIView : MonoBehaviour, IDevGUISection
    {
        [SerializeField]
        private Transform content;
        private Dictionary<string, IDevGUIElement> elements = new Dictionary<string, IDevGUIElement>();

        public void AddSelector<T>(string name, Func<T> get, Action<T> set, float step = 1f)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            if (elements.ContainsKey(name))
            {
                Debug.LogError($"Element {name} is already added.");
                return;
            }

            Type type = typeof(T);
            DevGUISelector selector = DevResources.InstantiateSelector<DevGUISelector>(type, content);
            if (selector == null)
            {
                Debug.LogError($"Type {type} is not support.");
                return;
            }

            if (selector is DevGUIValueSelector valueSelector)
            {
                valueSelector.Init(name, type, get, set, step);
            }
            else
            {
                selector.Init(name, type, get, set);
            }

            elements.Add(name, selector);
        }

        public void RemoveSelector(string name)
        {
            RemoveElement(name);
        }

        public void AddMethod(string name, Action method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T>(string name, Action<T> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2>(string name, Action<T1, T2> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3>(string name, Action<T1, T2, T3> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4, T5, T6, T7>(string name, Action<T1, T2, T3, T4, T5, T6, T7> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4, T5, T6, T7, T8>(string name, Action<T1, T2, T3, T4, T5, T6, T7, T8> method)
        {
            AddDelegate(name, method);
        }

        public void AddMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string name, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> method)
        {
            AddDelegate(name, method);
        }

        public void RemoveMethod(string name)
        {
            RemoveElement(name);
        }

        private void AddDelegate(string name, Delegate del)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            if (elements.ContainsKey(name))
            {
                Debug.LogError($"Element {name} is already added.");
                return;
            }

            if (del == null)
            {
                Debug.LogError($"Delegate {name} is null.");
                return;
            }

            ParameterInfo[] parameters = del.Method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                Type type = parameter.ParameterType;
                if (typeof(Delegate).IsAssignableFrom(type) || type == typeof(object) || type == typeof(Enum))
                {
                    Debug.LogError($"Type {parameter.ParameterType} is not support.");
                    return;
                }
            }

            DevGUIMethod method = DevResources.Instantiate<DevGUIMethod>("DevGUIMethod", content);
            method.Init(name, del);
            elements.Add(name, method);
        }

        private void RemoveElement(string name)
        {
            if (elements.TryGetValue(name, out IDevGUIElement element))
            {
                element.Destroy();
                elements.Remove(name);
            }
        }
    }
}
