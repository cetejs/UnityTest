using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public class GameWindow : EditorWindow
    {
        private List<SubWindow> subWindows;
        private int currentWindowIndex;

        private const int MaxHorizontalWindowCount = 5;

        [MenuItem("Window/Game Framework")]
        private static void OpenGameWindow()
        {
            GetWindow<GameWindow>("Game Framework");
        }

        private void InitializeWindows()
        {
            List<Type> subWindowTypes = AssemblyUtility.GetAssignableTypes(GameSetting.Instance.AssemblyNames, typeof(SubWindow));
            if (subWindows == null)
            {
                subWindows = new List<SubWindow>(subWindowTypes.Count);
                foreach (Type type in subWindowTypes)
                {
                    SubWindow window = (SubWindow)Activator.CreateInstance(type);
                    window.Init(ObjectNames.NicifyVariableName(type.Name), this);
                    subWindows.Add(window);
                }
            }
        }

        private void DrawWindowTabs()
        {
            if (subWindows.Count == 0)
            {
                return;
            }

            float tabWidth = position.width / Mathf.Min(subWindows.Count, MaxHorizontalWindowCount);
            for (int i = 0; i < subWindows.Count; i += MaxHorizontalWindowCount)
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (int j = i; j < i + MaxHorizontalWindowCount && j < subWindows.Count; j++)
                    {
                        GUIStyle buttonStyle = currentWindowIndex == j ? WindowStyles.MenuButtonSelected : WindowStyles.MenuButton;
                        if (GUILayout.Button(subWindows[j].Name, buttonStyle, GUILayout.Width(tabWidth)))
                        {
                            currentWindowIndex = j;
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            InitializeWindows();
        }

        private void OnDisable()
        {
            foreach (SubWindow window in subWindows)
            {
                window.OnDestroy();
            }
        }

        private void OnGUI()
        {
            DrawWindowTabs();

            if (currentWindowIndex < subWindows.Count)
            {
                subWindows[currentWindowIndex].OnGUI();
            }
        }
    }
}
