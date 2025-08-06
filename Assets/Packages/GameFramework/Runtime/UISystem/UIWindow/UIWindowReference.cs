using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework
{
    internal partial class UIWindowManager
    {
        private class UIWindowReference
        {
            private UIWindow prefab;
            private AsyncOperationHandle<GameObject> handle;

            public bool IsDone
            {
                get { return handle.IsDone; }
            }

            public UIWindowReference(string path)
            {
                path = PathUtility.CombineWithExtension(UISetting.Instance.WindowPath, path, ".prefab");
                handle = Addressables.LoadAssetAsync<GameObject>(path);
            }

            public UIWindow Get()
            {
                if (prefab != null)
                {
                    return prefab;
                }

                if (!handle.IsDone)
                {
                    handle.WaitForCompletion();
                }

                prefab = handle.Result.GetComponent<UIWindow>();
                prefab.gameObject.SetActive(false);
                return prefab;
            }

            public void GetAsync(Action<UIWindow> callback)
            {
                if (prefab != null)
                {
                    callback.Invoke(prefab);
                    return;
                }

                handle.Completed += _ =>
                {
                    prefab = handle.Result.GetComponent<UIWindow>();
                    prefab.gameObject.SetActive(false);
                    callback.Invoke(prefab);
                };
            }
        }
    }
}
