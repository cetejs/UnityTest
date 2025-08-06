using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public static class InspectorUtility
    {
        private const string EditorPrefsFoldoutKey = "GameFramework.Foldout";

        public static bool Foldout(Object obj, string label, string identifier = "")
        {
            return Foldout(obj, new GUIContent(label), identifier);
        }

        public static bool Foldout(Object obj, GUIContent content, string identifier = "")
        {
            string key = $"{EditorPrefsFoldoutKey}.{obj.GetType().FullName}.{identifier}.{content.text}";
            bool prevFoldout = EditorPrefs.GetBool(key);
            bool foldout = EditorGUILayout.Foldout(prevFoldout, content, true);
            if (foldout != prevFoldout)
            {
                EditorPrefs.SetBool(key, foldout);
            }

            return foldout;
        }

        public static bool ToggleFoldout(Object obj, string label, string identifier = "")
        {
            return ToggleFoldout(obj, new GUIContent(label), identifier);
        }

        public static bool ToggleFoldout(Object obj, GUIContent content, string identifier = "")
        {
            string key = $"{EditorPrefsFoldoutKey}.{obj.GetType().FullName}.{identifier}.{content.text}";
            bool prevFoldout = EditorPrefs.GetBool(key);
            bool foldout = GUILayout.Toggle(prevFoldout, content, EditorStyles.foldoutHeader);
            if (foldout != prevFoldout)
            {
                EditorPrefs.SetBool(key, foldout);
            }

            return foldout;
        }
    }
}
