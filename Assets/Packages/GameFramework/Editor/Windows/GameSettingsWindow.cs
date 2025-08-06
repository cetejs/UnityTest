using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class GameSettingsWindow : SubWindow
    {
        private Editor settingEditor;

        public override void Init(string name, GameWindow parent)
        {
            base.Init("GameSettings", parent);
            settingEditor = Editor.CreateEditor(GameSetting.Instance);
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }
    }
}
