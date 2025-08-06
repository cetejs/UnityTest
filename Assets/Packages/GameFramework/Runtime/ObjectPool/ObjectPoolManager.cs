using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFramework
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private Dictionary<object, ObjectPool> pools = new Dictionary<object, ObjectPool>();

        public static T Get<T>(object key, Transform parent = null) where T : PoolObject
        {
            return Instance?.GetObjectPoolInternal(key).Get<T>(parent);
        }

        public static void GetAsync<T>(object key, Action<PoolObject> callback) where T : PoolObject
        {
            GetAsync(key, null, callback);
        }

        public static void GetAsync<T>(object key, Transform parent, Action<T> callback) where T : PoolObject
        {
            Instance?.GetObjectPoolInternal(key).GetAsync<T>(parent, callback);
        }

        public static void Release<T>(T obj) where T : PoolObject
        {
            obj.Release();
        }

        public static void Add(object key, int count = 1)
        {
            Instance?.GetObjectPoolInternal(key).Add(count);
        }

        public static void Clear(object key)
        {
            Instance?.GetObjectPoolInternal(key).Clear();
        }

        public static void Clear()
        {
            if (Instance == null)
            {
                return;
            }

            foreach (ObjectPool pool in Instance.pools.Values)
            {
                pool.Clear();
            }

            Instance.pools.Clear();
        }

        public static ObjectPool GetObjectPool(object key)
        {
            return Instance?.GetObjectPoolInternal(key);
        }

        private ObjectPool GetObjectPoolInternal(object key)
        {
            if (key is IKeyEvaluator evaluator)
            {
                key = evaluator.RuntimeKey;
            }

            if (!pools.TryGetValue(key, out ObjectPool objectPool))
            {
                objectPool = new ObjectPool(key, transform);
                pools.Add(key, objectPool);
            }

            return objectPool;
        }
    }
}
