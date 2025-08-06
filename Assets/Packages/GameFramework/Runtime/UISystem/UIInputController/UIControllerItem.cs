using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework
{
    [Serializable]
    public class UIControllerItem
    {
        public string itemName;
        public AssetReferenceSprite itemIcon;
        public string itemText;

        private AsyncOperationHandle<Sprite> handle;

        public AsyncOperationHandle<Sprite> LoadSprite()
        {
            if (handle.IsValid())
            {
                return handle;
            }

            handle = Addressables.LoadAssetAsync<Sprite>(itemIcon);
            return handle;
        }

        public void Release()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
