using System;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Escape
{
    public class UIKeyNumber : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        private int num;
        private int remain;

        private void OnEnable()
        {
            EventManager.Register<GenericData<int>>((int) EventId.KeyNumber, OnKeyNumber);
            EventManager.Register((int) EventId.KeyCount, OnKeyCount);
        }

        private void OnDisable()
        {
            EventManager.Unregister<GenericData<int>>((int) EventId.KeyNumber, OnKeyNumber);
            EventManager.Unregister((int) EventId.KeyCount, OnKeyCount);
        }

        private void OnKeyNumber(GenericData<int> data)
        {
            num = remain = data.Item;
            text.text = string.Concat(num, "/", num);
        }
        
        private void OnKeyCount()
        {
            text.text = string.Concat(--remain, "/", num);
            if (remain == 0)
            {
                EventManager.Send((int) EventId.OpenDoor);
            }
        }
    }
}