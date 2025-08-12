using BehaviorDesigner;
using GameFramework;
using UnityEngine;

namespace Escape
{
    public class RestartBehavior : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTree behavior;

        private void Start()
        {
            behavior = GetComponent<BehaviorTree>();
            EventManager.Register((int) EventId.Reborn, Reborn);
        }

        private void OnDestroy()
        {
            EventManager.Unregister((int) EventId.Reborn, Reborn);
        }

        private void Reborn()
        {
            DelayManager.Add(behavior.Restart, 0.5f);
        }
    }
}