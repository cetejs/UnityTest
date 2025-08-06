using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevGUIJsonSelector : DevGUISelector
    {
        [SerializeField]
        private InputField inputField;
        private string value;

        private void Awake()
        {
            inputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        private void OnEnable()
        {
            value = JsonUtility.ConvertToJson(get.DynamicInvoke(), type);
            inputField.SetTextWithoutNotify(value);
        }

        private void OnInputEndEdit(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (JsonUtility.TryCovertToObject(text, type, out object result))
                {
                    value = text;
                    set.DynamicInvoke(result);
                }
                else
                {
                    inputField.SetTextWithoutNotify(value);
                }
            }
            else
            {
                value = null;
                set.DynamicInvoke(value);
            }
        }
    }
}
