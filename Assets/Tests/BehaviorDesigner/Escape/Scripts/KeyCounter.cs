using GameFramework;
using UnityEngine;

namespace Escape
{
    public class KeyCounter : MonoBehaviour
    {
        private void Start()
        {
            EventManager.Send((int) EventId.KeyNumber, new GenericData<int>()
            {
                Item = transform.childCount
            });
        }
    }
}