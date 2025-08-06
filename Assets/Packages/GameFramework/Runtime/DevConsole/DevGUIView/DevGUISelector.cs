using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public abstract class DevGUISelector : MonoBehaviour, IDevGUIElement
    {
        [SerializeField]
        private Text text;
        protected Type type;
        protected Delegate get;
        protected Delegate set;

        public virtual void Init(string name, Type type, Delegate get, Delegate set)
        {
            this.name = name;
            this.type = type;
            this.get = get;
            this.set = set;
            text.text = name;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
