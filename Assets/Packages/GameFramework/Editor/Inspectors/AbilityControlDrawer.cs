using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameFramework
{
    public class AbilityControlDrawer
    {
        private ReorderableList abilityList;
        private SerializedObject serializedObject;
        private SerializedProperty property;
        private Type abilityBaseType;

        private static Dictionary<Type, string> abilityNames = new Dictionary<Type, string>();

        public AbilityControlDrawer(SerializedProperty property, Type abilityBaseType)
        {
            serializedObject = property.serializedObject;
            this.property = property;
            this.abilityBaseType = abilityBaseType;
            InitReorderableList();
        }

        private string GetAbilityName(Type type)
        {
            if (!abilityNames.TryGetValue(type, out string abilityName))
            {
                AbilityNameAttribute abilityNameAttribute = type.GetCustomAttribute<AbilityNameAttribute>();
                if (abilityNameAttribute != null)
                {
                    abilityName = abilityNameAttribute.Name;
                }
                else
                {
                    abilityName = type.Name;
                }

                abilityNames.Add(type, abilityName);
            }

            return abilityName;
        }

        private void InitReorderableList()
        {
            GenericMenu addMenu;
            List<Type> allTypes = AssemblyUtility.GetAssignableTypes(GameSetting.Instance.AssemblyNames, abilityBaseType);
            abilityList = new ReorderableList(serializedObject, property)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, abilityBaseType.Name);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty serializedProperty = property.GetArrayElementAtIndex(index);
                    if (serializedProperty.managedReferenceValue == null)
                    {
                        EditorGUI.LabelField(rect, $"NullReferenceException");
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, GetAbilityName(serializedProperty.managedReferenceValue.GetType()));
                        SerializedProperty isEnabledProperty = serializedProperty.FindPropertyRelative("isEnabled");
                        if (isEnabledProperty != null)
                        {
                            rect.x += rect.width - 30;
                            rect.width = 30;
                            EditorGUI.PropertyField(rect, isEnabledProperty, GUIContent.none);
                        }
                    }
                },
                onAddCallback = list =>
                {
                    addMenu = new GenericMenu();
                    foreach (Type type in allTypes)
                    {
                        bool found = false;
                        for (int i = 0; i < property.arraySize; i++)
                        {
                            if (type == property.GetArrayElementAtIndex(i).managedReferenceValue.GetType())
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            string abilityName;
                            AbilityNameAttribute abilityNameAttribute = type.GetCustomAttribute<AbilityNameAttribute>();
                            if (abilityNameAttribute != null)
                            {
                                abilityName = abilityNameAttribute.Name;
                            }
                            else
                            {
                                abilityName = type.Name;
                            }

                            addMenu.AddItem(new GUIContent(abilityName), false, () =>
                            {
                                property.InsertArrayElementAtIndex(list.count);
                                property.GetArrayElementAtIndex(list.count - 1).managedReferenceValue = Activator.CreateInstance(type);
                                serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }

                    addMenu.ShowAsContext();
                },
                onRemoveCallback = list =>
                {
                    property.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                    list.index = Mathf.Max(list.index - 1, 0);
                }
            };
        }

        public void DrawAbilities()
        {
            if (InspectorUtility.ToggleFoldout(property.serializedObject.targetObject, property.displayName))
            {
                abilityList.DoLayoutList();
                if (abilityList.count > 0 && abilityList.IsSelected(abilityList.index))
                {
                    SerializedProperty selectedAbility = property.GetArrayElementAtIndex(abilityList.index);
                    if (selectedAbility.managedReferenceValue == null)
                    {
                        return;
                    }

                    Type abilityType = selectedAbility.managedReferenceValue.GetType();
                    EditorGUILayout.LabelField(GetAbilityName(abilityType), EditorStyles.boldLabel);
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        int depth = selectedAbility.depth;
                        for (bool enterChildren = true; selectedAbility.NextVisible(enterChildren) && selectedAbility.depth > depth; enterChildren = false)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(selectedAbility, new GUIContent(selectedAbility.displayName), true);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }
    }
}
