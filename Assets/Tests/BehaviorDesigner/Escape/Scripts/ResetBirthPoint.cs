using GameFramework;
using UnityEngine;

namespace Escape
{
    public class ResetBirthPoint : MonoBehaviour
    {
        [SerializeField]
        private Vector3 birthPosition;
        [SerializeField]
        private Quaternion birthRotation;

        private void Start()
        {
            birthPosition = transform.position;
            birthRotation = transform.rotation;
            EventManager.Register((int) EventId.Reborn, Reborn);
        }

        private void OnDestroy()
        {
            EventManager.Unregister((int) EventId.Reborn, Reborn);
        }

        private void Reborn()
        {
            transform.position = birthPosition;
            transform.rotation = birthRotation;
        }
    }
}