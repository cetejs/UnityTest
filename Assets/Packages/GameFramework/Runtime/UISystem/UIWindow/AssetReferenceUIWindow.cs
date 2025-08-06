using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFramework
{
    [Serializable]
    public class AssetReferenceUIWindow : AssetReferenceT<GameObject>
    {
        public AssetReferenceUIWindow(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(string mainAssetPath)
        {
#if UNITY_EDITOR
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(mainAssetPath);
            if (go != null && go.TryGetComponent(out UIWindow _))
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
