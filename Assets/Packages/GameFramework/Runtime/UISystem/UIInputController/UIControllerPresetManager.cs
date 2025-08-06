using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    [CreateAssetMenu(menuName = "GameFramework/Controller Preset Manager")]
    public class UIControllerPresetManager : ScriptableObject
    {
        public UIControllerPreset KeyboardMousePreset;
        public UIControllerPreset XboxControllerPreset;
        public UIControllerPreset PlayStationControllerPreset;
        public List<UIControllerPreset> CustomControllerPresets = new List<UIControllerPreset>();
    }
}
