using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class DevGUIBoolSelector : DevGUISelector
    {
        [SerializeField]
        private Toggle toggle;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(isOn => { set.DynamicInvoke(isOn); });
        }

        private void OnEnable()
        {
            toggle.SetIsOnWithoutNotify((bool)get.DynamicInvoke());
        }
    }
}
