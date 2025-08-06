using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevGUIValueSelector : DevGUICountSelector
    {
        [SerializeField]
        private InputField inputField;
        private double value;
        private float step;

        private void Awake()
        {
            inputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        private void OnEnable()
        {
            value = Convert.ToDouble(get.DynamicInvoke());
            value = Math.Round(value, 2);
            inputField.SetTextWithoutNotify(value.ToString());
        }

        private void OnInputEndEdit(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (JsonUtility.TryCovertToObject(text, type, out object result))
                {
                    value = Convert.ToDouble(result);
                    value = Math.Round(value, 2);
                    result = CovertUtility.ChangeType(value, type);
                    set.DynamicInvoke(result);
                }
            }

            inputField.SetTextWithoutNotify(value.ToString());
        }

        public virtual void Init(string name, Type type, Delegate get, Delegate set, float step)
        {
            base.Init(name, type, get, set);
            if (type != typeof(float) &&
                type != typeof(double))
            {
                this.step = Mathf.Max(1, Mathf.CeilToInt(step));
                inputField.characterValidation = InputField.CharacterValidation.Integer;
            }
            else
            {
                this.step = step;
                inputField.characterValidation = InputField.CharacterValidation.Decimal;
            }
        }

        protected override void OnCountValue(bool plus)
        {
            if (plus)
            {
                value += step;
            }
            else
            {
                value -= step;
            }

            object result = CovertUtility.ChangeType(value, type);
            value = Convert.ToDouble(result);
            value = Math.Round(value, 2);
            set.DynamicInvoke(result);
            inputField.SetTextWithoutNotify(value.ToString());
        }

        private void SetText(string text)
        {
            inputField.SetTextWithoutNotify(text);
        }
    }
}
