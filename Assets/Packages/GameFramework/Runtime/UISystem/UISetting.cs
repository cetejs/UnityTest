using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace GameFramework
{
    public class UISetting : ScriptableObjectSingleton<UISetting>
    {
        public string WindowPath = "Assets/Prefabs/Windows";
        public CanvasSetting CanvasSettings = new CanvasSetting()
        {
            ReferenceResolution = new Vector2(1920f, 1080f),
            ScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize,
            ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
            Match = 0
        };
        public WindowLayer[] WindowLayers = new WindowLayer[]
        {
            new WindowLayer()
            {
                Name = "FullScreen",
                Sort = 0
            },
            new WindowLayer()
            {
                Name = "Fix",
                Sort = 1000
            },
            new WindowLayer()
            {
                Name = "PopUp",
                Sort = 2000
            },
            new WindowLayer()
            {
                Name = "Top",
                Sort = 3000
            }
        };

        public UIControllerPresetManager ControllerPresetManager;

        [Serializable]
        public struct WindowLayer
        {
            public string Name;
            public int Sort;
        }

        [Serializable]
        public struct CanvasSetting
        {
            public Vector2 ReferenceResolution;
            public CanvasScaler.ScaleMode ScaleMode;
            public CanvasScaler.ScreenMatchMode ScreenMatchMode;
            [Range(0, 1)]
            public float Match;
        }
    }
}
