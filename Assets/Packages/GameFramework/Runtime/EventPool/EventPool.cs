using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class EventPool<TKey>
    {
        private MultiDictionary<TKey, Delegate> eventHandlers;
        private Queue<Wrapper> events;

        public EventPool()
        {
            eventHandlers = new MultiDictionary<TKey, Delegate>();
            events = new Queue<Wrapper>();
        }

        public EventPool(int capacity)
        {
            eventHandlers = new MultiDictionary<TKey, Delegate>(capacity);
            events = new Queue<Wrapper>(capacity);
        }

        public void Register(TKey id, Action handler)
        {
            RegisterInternal(id, handler);
        }

        public void Register<T>(TKey id, Action<T> handler) where T : IGenericData
        {
            RegisterInternal(id, handler);
        }

        public void Unregister(TKey id, Action handler)
        {
            UnregisterInternal(id, handler);
        }

        public void Unregister<T>(TKey id, Action<T> handler) where T : IGenericData
        {
            UnregisterInternal(id, handler);
        }

        public void Send(TKey id)
        {
            HandleEvent(id);
        }

        public void Send<T>(TKey id, T data) where T : IGenericData
        {
            HandleEvent(id, data);
        }

        public void SendAsync(TKey id)
        {
            lock (events)
            {
                events.Enqueue(new Wrapper()
                {
                    id = id
                });
            }
        }

        public void SendAsync<T>(TKey id, T data) where T : IGenericData
        {
            lock (events)
            {
                events.Enqueue(new Wrapper()
                {
                    id = id,
                    data = data
                });
            }
        }

        private void RegisterInternal(TKey id, Delegate handler)
        {
            if (handler == null)
            {
                Debug.LogError($"Event {id} handler is invalid");
            }

#if UNITY_EDITOR
            if (eventHandlers.Contains(id, handler))
            {
                Debug.LogError($"Event {id} is already subscribed");
                return;
            }
#endif
            eventHandlers.Add(id, handler);
        }

        private void UnregisterInternal(TKey id, Delegate handler)
        {
            if (handler == null)
            {
                Debug.LogError($"Event {id} handler is invalid");
                return;
            }

            if (!eventHandlers.Remove(id, handler))
            {
                Debug.LogError($"Event {id} not exist specified handler");
            }
        }

        private void HandleEvent(TKey id)
        {
            if (eventHandlers.TryGetValue(id, out LinkedListRange<Delegate> range))
            {
                foreach (Delegate handler in range)
                {
                    try
                    {
                        ((Action)handler).Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        private void HandleEvent<T>(TKey id, T data) where T : IGenericData
        {
            if (eventHandlers.TryGetValue(id, out LinkedListRange<Delegate> range))
            {
                foreach (Delegate handler in range)
                {
                    try
                    {
                        ((Action<T>)handler).Invoke(data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        public void Update()
        {
            lock (events)
            {
                while (events.Count != 0)
                {
                    Wrapper ent = events.Dequeue();
                    HandleEvent(ent.id, ent.data);
                }
            }
        }

        public void Clear()
        {
            lock (events)
            {
                events.Clear();
                eventHandlers.Clear();
            }
        }

        private struct Wrapper
        {
            public TKey id;
            public IGenericData data;
        }
    }
}
