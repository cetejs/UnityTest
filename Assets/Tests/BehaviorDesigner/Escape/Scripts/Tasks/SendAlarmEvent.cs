using BehaviorDesigner;
using GameFramework;
using UnityEngine;

namespace Escape
{
    [TaskGroup("Escape")]
    public class SendAlarmEvent : Action
    {
        [SerializeField]
        private EventId eventId;
        [SerializeField]
        private SharedFloat radius;
        [SerializeField]
        private SharedTransform warner;
        
        public override TaskStatus OnUpdate()
        {
            EventManager.Send((int)eventId, new GenericData<float, Transform>()
            {
                Item1 = radius.Value,
                Item2 = warner.Value
            });
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            eventId = 0;
            radius = 0f;
            warner = null;
        }
    }
}