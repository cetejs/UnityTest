using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public static class WindowStyles
    {
        public static GUIStyle MenuButton;
        public static GUIStyle MenuButtonSelected;

        static WindowStyles()
        {
            MenuButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Normal,
                fontSize = 14,
                fixedHeight = 24
            };

            MenuButtonSelected = new GUIStyle(MenuButton)
            {
                fontStyle = FontStyle.Bold
            };
        }
    }
}
