using BehaviorDesigner;
using GameFramework;
using UnityEngine;

namespace Escape
{
    [TaskGroup("Escape")]
    public class ReceiveAlarmEvent : Action
    {
        [SerializeField]
        private EventId eventId;
        [SerializeField]
        private SharedFloat radius;
        [SerializeField]
        private SharedTransform warner;

        public override void OnStart()
        {
            base.OnStart();
            EventManager.Register<GenericData<float, Transform>>((int)eventId, EventHandler);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            EventManager.Unregister<GenericData<float, Transform>>((int)eventId, EventHandler);
        }

        private void EventHandler(GenericData<float, Transform> data)
        {
            radius.Value = data.Item1;
            warner.Value = data.Item2;
        }
        
        public override void OnReset()
        {
            eventId = 0;
            radius = 0f;
            warner = null;
        }
    }
}