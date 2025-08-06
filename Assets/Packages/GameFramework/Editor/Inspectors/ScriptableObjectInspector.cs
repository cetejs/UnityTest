using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), true)]
    public class ScriptableObjectInspector : Editor
    {
        private InspectorGroupDrawer groupDrawer;

        public override void OnInspectorGUI()
        {
            if (groupDrawer == null)
            {
                groupDrawer = new InspectorGroupDrawer(serializedObject);
            }

            if (groupDrawer.HasGroup)
            {
                groupDrawer.DrawGroupInspector();
            }
            else
            {
                DrawDefaultInspector();
            }
        }
    }
}
