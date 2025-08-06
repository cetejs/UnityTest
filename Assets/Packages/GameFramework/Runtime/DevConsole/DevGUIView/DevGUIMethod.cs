using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevGUIMethod : MonoBehaviour, IDevGUIElement
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Button button;
        private Delegate del;
        private DevGUISelector[] selectors;
        private object[] parms;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                del.DynamicInvoke(parms);
            });
        }

        public void Init(string name, Delegate del)
        {
            text.text = name;
            this.name = name;
            this.del = del;
            ParameterInfo[] parameters = del.Method.GetParameters();
            selectors = new DevGUISelector[parameters.Length];
            parms = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                int j = i;
                Type type = parameters[i].ParameterType;
                Func<object> get = () => parameters[j].DefaultValue;
                Action<object> set = value => parms[j] = value;
                selectors[i] = DevResources.InstantiateSelector<DevGUISelector>(type, transform);
                selectors[i].Init(parameters[i].Name, type, get, set);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
