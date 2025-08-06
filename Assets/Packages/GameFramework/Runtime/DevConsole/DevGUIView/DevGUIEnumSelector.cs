using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevGUIEnumSelector : DevGUICountSelector
    {
        [SerializeField]
        private Text label;
        private Array array;
        private int index;

        private void OnEnable()
        {
            object value = get.DynamicInvoke();
            for (int i = 0; i < array.Length; i++)
            {
                if (array.GetValue(i).Equals(value))
                {
                    index = i;
                    break;
                }
            }

            label.text = value.ToString();
        }

        public override void Init(string name, Type type, Delegate get, Delegate set)
        {
            base.Init(name, type, get, set);
            array = Enum.GetValues(type);
        }

        protected override void OnCountValue(bool plus)
        {
            if (plus)
            {
                if (++index == array.Length)
                {
                    index = 0;
                }
            }
            else
            {
                if (--index == 0)
                {
                    index = array.Length - 1;
                }
            }

            object value = array.GetValue(index);
            set.DynamicInvoke(value);
            label.text = value.ToString();
        }
    }
}
