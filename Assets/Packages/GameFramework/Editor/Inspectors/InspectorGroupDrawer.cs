using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public class InspectorGroupDrawer
    {
        private SerializedObject serializedObject;
        private Dictionary<string, GroupData> groupDataMap = new Dictionary<string, GroupData>();
        private List<SerializedProperty> properties = new List<SerializedProperty>();
        private Dictionary<string, AbilityControlDrawer> abilityDrawerMap = new Dictionary<string, AbilityControlDrawer>();

        public InspectorGroupDrawer(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            CollectGroupData();
        }

        public bool HasGroup
        {
            get { return groupDataMap.Count > 0 || abilityDrawerMap.Count > 0; }
        }

        private void CollectGroupData()
        {
            groupDataMap.Clear();
            properties.Clear();
            abilityDrawerMap.Clear();
            List<FieldInfo> infos = AssemblyUtility.GetFieldInfos(serializedObject.targetObject.GetType());
            List<FieldInfo> abilityInfos = new List<FieldInfo>();
            InspectorGroupAttribute previousGroupAttribute = null;
            foreach (FieldInfo fieldInfo in infos)
            {
                if (fieldInfo.IsDefined(typeof(HideInInspector), true))
                {
                    continue;
                }

                if (fieldInfo.IsDefined(typeof(AbilityControlAttribute), true))
                {
                    abilityInfos.Add(fieldInfo);
                }

                if (fieldInfo.IsDefined(typeof(IgnoreInspectorGroupAttribute), true))
                {
                    continue;
                }

                InspectorGroupAttribute groupAttribute = fieldInfo.GetCustomAttribute<InspectorGroupAttribute>();
                if (groupAttribute == null && previousGroupAttribute != null && previousGroupAttribute.GroupAllFieldsUntilNextGroupAttribute)
                {
                    groupDataMap[previousGroupAttribute.GroupName].FieldNames.Add(fieldInfo.Name);
                }

                if (groupAttribute != null)
                {
                    previousGroupAttribute = groupAttribute;
                    if (!groupDataMap.TryGetValue(groupAttribute.GroupName, out GroupData groupData))
                    {
                        groupData = new GroupData()
                        {
                            GroupAttribute = groupAttribute,
                            FieldNames = new HashSet<string>(),
                            Properties = new List<SerializedProperty>()
                        };

                        groupDataMap.Add(groupAttribute.GroupName, groupData);
                    }

                    groupData.FieldNames.Add(fieldInfo.Name);
                }
            }

            if (groupDataMap.Count > 0 || abilityInfos.Count > 0)
            {
                SerializedProperty iterator = serializedObject.GetIterator();
                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    bool hasGroup = false;
                    foreach (GroupData data in groupDataMap.Values)
                    {
                        if (data.FieldNames.Contains(iterator.name))
                        {
                            data.Properties.Add(iterator.Copy());
                            hasGroup = true;
                            break;
                        }
                    }

                    if (!hasGroup)
                    {
                        properties.Add(iterator.Copy());
                    }

                    foreach (FieldInfo fieldInfo in abilityInfos)
                    {
                        if (fieldInfo.Name == iterator.name)
                        {
                            Type abilityBaseType;
                            if (fieldInfo.FieldType.IsArray)
                            {
                                abilityBaseType = fieldInfo.FieldType.GetElementType();
                            }
                            else
                            {
                                abilityBaseType = fieldInfo.FieldType.GenericTypeArguments[0];
                            }

                            abilityDrawerMap.Add(fieldInfo.Name, new AbilityControlDrawer(iterator.Copy(), abilityBaseType));
                            break;
                        }
                    }
                }
            }
        }

        public void DrawGroupInspector()
        {
            if (!HasGroup)
            {
                return;
            }

            if (properties.Count > 0 && properties[0].propertyPath == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(properties[0]);
                }
            }

            serializedObject.UpdateIfRequiredOrScript();
            foreach (GroupData groupData in groupDataMap.Values)
            {
                if (InspectorUtility.ToggleFoldout(serializedObject.targetObject, groupData.GroupAttribute.GroupName))
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        foreach (SerializedProperty property in groupData.Properties)
                        {
                            if (abilityDrawerMap.TryGetValue(property.name, out AbilityControlDrawer abilityDrawer))
                            {
                                abilityDrawer.DrawAbilities();
                            }
                            else
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(property);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < properties.Count; i++)
            {
                SerializedProperty property = properties[i];
                if (abilityDrawerMap.TryGetValue(property.name, out AbilityControlDrawer abilityDrawer))
                {
                    abilityDrawer.DrawAbilities();
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property);
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private class GroupData
        {
            public InspectorGroupAttribute GroupAttribute;
            public List<SerializedProperty> Properties = new List<SerializedProperty>();
            public HashSet<string> FieldNames = new HashSet<string>();
        }
    }
}
