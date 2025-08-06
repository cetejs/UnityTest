using System;
using UnityEngine;

namespace GameFramework
{
    public class ObjectPool<T> where T : PoolObject
    {
        private ObjectPool pool;

        public ObjectPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public ObjectPool(PoolObject prefab, Transform root)
        {
            pool = new ObjectPool(prefab, root);
        }

        public T Get(Transform parent = null)
        {
            return pool.Get<T>(parent);
        }

        public void GetAsync(Action<T> callback)
        {
            pool.GetAsync(null, callback);
        }

        public void GetAsync(Transform parent, Action<T> callback)
        {
            pool.GetAsync(parent, callback);
        }

        public void Release(T obj)
        {
            pool.Release(obj);
        }

        public void Add(int count = 1)
        {
            pool.Add(count);
        }

        public void Clear()
        {
            pool.Clear();
        }
    }
}
