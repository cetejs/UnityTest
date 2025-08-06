using UnityEngine.AddressableAssets;

namespace GameFramework
{
    public class UISetting : ScriptableObjectSingleton<UISetting>
    {
        public string WindowPath = "Assets/Prefabs/Windows";
        public AssetReferenceGameObject WindowRoot;
        public string[] WindowLayers = new string[]
        {
            "FullScreen",
            "Fix",
            "PopUp",
            "Top"
        };

        public UIControllerPresetManager ControllerPresetManager;
    }
}
