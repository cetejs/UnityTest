using System;

namespace GameFramework
{
    public class EventManager : MonoSingleton<EventManager>
    {
        private EventPool<int> eventPool = new EventPool<int>();

        private void LateUpdate()
        {
            eventPool.Update();
        }

        public void Register(int id, Action handler)
        {
            Instance?.eventPool.Register(id, handler);
        }

        public void Register<T>(int id, Action<T> handler) where T : IGenericData
        {
            Instance?.eventPool.Register(id, handler);
        }

        public void Unregister(int id, Action handler)
        {
            Instance?.eventPool.Unregister(id, handler);
        }

        public void Unregister<T>(int id, Action<T> handler) where T : IGenericData
        {
            Instance?.eventPool.Unregister(id, handler);
        }

        public void Send(int id)
        {
            Instance?.eventPool.Send(id);
        }

        public void Send<T>(int id, T data) where T : IGenericData
        {
            Instance?.eventPool.Send(id, data);
        }

        public void SendAsync(int id)
        {
            Instance?.eventPool.SendAsync(id);
        }

        public void SendAsync<T>(int id, T data) where T : IGenericData
        {
            Instance?.eventPool.SendAsync(id, data);
        }

        public void Clear()
        {
            eventPool.Clear();
        }
    }
}
