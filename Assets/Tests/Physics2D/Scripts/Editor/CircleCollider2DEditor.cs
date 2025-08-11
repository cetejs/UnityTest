using UnityEditor;
using UnityEngine;

namespace Tests
{
    [CustomEditor(typeof(CustomCircleCollider2D), true)]
    public class CircleCollider2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            CustomCircleCollider2D col = target as CustomCircleCollider2D;
            Vector3 offset = EditorGUILayout.Vector2Field("Offset", col.bounds.center);
            float radius = EditorGUILayout.FloatField("Radius", col.Radius);
            radius = Mathf.Max(radius, 0);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(col, "Update Collider2D");
                col.bounds.center = offset;
                col.Radius = radius;
                SceneView.RepaintAll();
            }
        }
    }
}