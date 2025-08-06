using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourInspector : Editor
    {
        private InspectorGroupDrawer groupDrawer;

        public override VisualElement CreateInspectorGUI()
        {
            groupDrawer = new InspectorGroupDrawer(serializedObject);
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
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
