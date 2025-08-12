using System;
using GameFramework;
using UnityEngine;

namespace Escape
{
    public class Door : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Register((int)EventId.OpenDoor, OnOpenDoor);
        }

        private void OnDisable()
        {
            EventManager.Unregister((int)EventId.OpenDoor, OnOpenDoor);
        }

        private void OnOpenDoor()
        {
            gameObject.SetActive(false);
        }
    }
}

