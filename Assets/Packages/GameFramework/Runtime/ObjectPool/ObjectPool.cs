using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GameFramework
{
    public class ObjectPool
    {
        private object key;
        private Transform root;
        private PoolObject prefab;
        private Stack<PoolObject> objects = new Stack<PoolObject>();
        private AsyncOperationHandle<GameObject> handle;

        public ObjectPool(object key, Transform root)
        {
            this.key = key;
            this.root = root;
        }

        public ObjectPool(PoolObject prefab, Transform root)
        {
            this.prefab = prefab;
            this.root = root;
        }

        private void TryLoadPrefab()
        {
            if (prefab == null)
            {
                handle = Addressables.LoadAssetAsync<GameObject>(key);
                handle.WaitForCompletion();
                GameObject go = handle.Result;
                if (!go.TryGetComponent(out prefab))
                {
                    prefab = go.AddComponent<EmptyPoolObject>();
                    Debug.LogError($"Pool object {go.name} not have {typeof(PoolObject)}");
                }
            }
        }

        private void TryLoadAsyncPrefab(Action callback)
        {
            if (!handle.IsValid())
            {
                handle = Addressables.LoadAssetAsync<GameObject>(key);
            }

            handle.Completed += _ =>
            {
                if (prefab == null)
                {
                    GameObject go = handle.Result;
                    if (!go.TryGetComponent(out prefab))
                    {
                        prefab = go.AddComponent<EmptyPoolObject>();
                        Debug.LogError($"Pool object {go.name} not have {typeof(PoolObject)}");
                    }
                }

                callback.Invoke();
            };
        }

        public T Get<T>(Transform parent) where T : PoolObject
        {
            if (prefab == null)
            {
                TryLoadPrefab();
            }

            return InstantiateObject<T>(parent);
        }

        public void GetAsync<T>(Transform parent, Action<T> callback) where T : PoolObject
        {
            if (prefab == null)
            {
                TryLoadAsyncPrefab(() => { callback.Invoke(InstantiateObject<T>(parent)); });
            }
            else
            {
                callback.Invoke(InstantiateObject<T>(parent));
            }
        }

        private T InstantiateObject<T>(Transform parent) where T : PoolObject
        {
            T obj;
            if (objects.Count > 0)
            {
                obj = objects.Pop() as T;
            }
            else
            {
                obj = Object.Instantiate(prefab, parent) as T;
                obj.Init(this);
            }

            obj.transform.SetParent(parent);
            obj.gameObject.SetActive(true);
            obj.WakeUp();
            return obj;
        }

        public void Release<T>(T obj) where T : PoolObject
        {
            obj.transform.SetParent(root);
            obj.gameObject.SetActive(false);
            obj.Sleep();
            objects.Push(obj);
        }

        public void Add(int count)
        {
            TryLoadPrefab();
            while (count-- > 0)
            {
                objects.Push(Object.Instantiate(prefab, root));
            }
        }

        public void Clear()
        {
            while (objects.Count > 0)
            {
                Object.Destroy(objects.Pop());
            }

            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            objects.Clear();
        }
    }
}
