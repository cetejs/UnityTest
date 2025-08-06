using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFramework
{
    [Serializable]
    public class AssetReferencePoolObject : AssetReferenceT<GameObject>
    {
        public AssetReferencePoolObject(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(string mainAssetPath)
        {
#if UNITY_EDITOR
            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(mainAssetPath);
            if (go != null && go.TryGetComponent(out PoolObject _))
            {
                return true;
            }

            return false;
#else
            return false;
#endif
        }
    }
}
