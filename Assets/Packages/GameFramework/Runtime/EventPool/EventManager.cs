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

        public static void Register(int id, Action handler)
        {
            Instance?.eventPool.Register(id, handler);
        }

        public static void Register<T>(int id, Action<T> handler) where T : IGenericData
        {
            Instance?.eventPool.Register(id, handler);
        }

        public static void Unregister(int id, Action handler)
        {
            Instance?.eventPool.Unregister(id, handler);
        }

        public static void Unregister<T>(int id, Action<T> handler) where T : IGenericData
        {
            Instance?.eventPool.Unregister(id, handler);
        }

        public static void Send(int id)
        {
            Instance?.eventPool.Send(id);
        }

        public static void Send<T>(int id, T data) where T : IGenericData
        {
            Instance?.eventPool.Send(id, data);
        }

        public static void SendAsync(int id)
        {
            Instance?.eventPool.SendAsync(id);
        }

        public static void SendAsync<T>(int id, T data) where T : IGenericData
        {
            Instance?.eventPool.SendAsync(id, data);
        }

        public static void Clear()
        {
            Instance?.eventPool.Clear();
        }
    }
}
