using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Toggle))]
    public class UIToggleSwitch : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> SwitchOn = new List<GameObject>();
        [SerializeField]
        private List<GameObject> SwitchOff = new List<GameObject>();
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            OnValueChanged(toggle.isOn);
        }

        private void OnValueChanged(bool isOn)
        {
            foreach (GameObject obj in SwitchOn)
            {
                obj.SetActive(isOn);
            }

            foreach (GameObject obj in SwitchOff)
            {
                obj.SetActive(!isOn);
            }
        }
    }
}
