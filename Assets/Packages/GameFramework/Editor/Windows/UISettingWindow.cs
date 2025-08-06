using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class UISettingWindow : SubWindow
    {
        private Editor settingEditor;

        public override void Init(string name, GameWindow parent)
        {
            base.Init("UISettingWindow", parent);
            settingEditor = Editor.CreateEditor(UISetting.Instance);
        }

        public override void OnGUI()
        {
            settingEditor.OnInspectorGUI();
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }
    }
}
