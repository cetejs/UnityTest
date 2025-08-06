using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevGUITable : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private GameObject view;

        [SerializeField]
        private Color selectColor;
        [SerializeField]
        private Color unselectColor;

        private void Awake()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            OnValueChanged(toggle.isOn);
        }

        private void OnValueChanged(bool isOn)
        {
            text.color = isOn ? unselectColor : selectColor;
            toggle.image.color = isOn ? selectColor : unselectColor;
            view.gameObject.SetActive(isOn);
        }

        public void Init(string name, GameObject view)
        {
            text.text = name;
            this.name = name;
            this.view = view;
        }
    }
}
