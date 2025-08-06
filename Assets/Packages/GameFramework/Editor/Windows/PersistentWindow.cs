using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class PersistentWindow : SubWindow
    {
        private Editor settingEditor;
        private PersistentData newData = new PersistentData();
        private bool showNewDataBox;
        private string storageName;
        private IPersistentStorage storage;

        private List<string> allKeys = new List<string>();
        private List<PersistentData> dataList = new List<PersistentData>();

        public override void Init(string name, GameWindow parent)
        {
            base.Init("PersistentData", parent);
            settingEditor = Editor.CreateEditor(PersistentSetting.Instance);
            storageName = PersistentSetting.Instance.DefaultStorageName;
            storage = PersistentManager.GetStorage(storageName);
            RefreshData();
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            using (new GUILayout.HorizontalScope())
            {
                storageName = EditorGUILayout.TextField("Storage Name", storageName);
                if (GUILayout.Button("Load", GUILayout.Width(100)))
                {
                    PersistentManager.Unload(storageName);
                    storage = PersistentManager.GetStorage(storageName);
                    RefreshData();
                }
            }

            for (int i = 0; i < dataList.Count; i++)
            {
                DrawData(dataList[i]);
            }

            if (showNewDataBox)
            {
                DrawNewData();
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("New Data"))
                {
                    NewData();
                }

                if (GUILayout.Button("Save"))
                {
                    storage.Save();
                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("Delete"))
                {
                    Delete();
                }
            }
        }

        private void RefreshData()
        {
            dataList.Clear();
            storage.GetAllKeys(allKeys);
            foreach (string key in allKeys)
            {
                PersistentData localData = new PersistentData
                {
                    Key = key
                };

                try
                {
                    localData.Value = storage.GetData<string>(key, default);
                }
                catch (Exception)
                {
                    localData.Value = null;
                }

                dataList.Add(localData);
            }
        }

        private void DrawData(PersistentData data)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (data.Value == null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(data.Key, "This type of display is not supported");
                }
                else
                {
                    data.Value = EditorGUILayout.TextField(data.Key, data.Value);
                }

                if (GUILayout.Button("Set", GUILayout.Width(100)))
                {
                    storage.SetData(data.Key, data.Value);
                }

                GUI.enabled = true;
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    storage.DeleteKey(data.Key);
                    dataList.Remove(data);
                }
            }
        }

        private void NewData()
        {
            newData.Key = default;
            newData.Value = default;
            showNewDataBox = true;
        }

        private void DrawNewData()
        {
            using (new GUILayout.HorizontalScope())
            {
                newData.Key = EditorGUILayout.TextField(newData.Key);
                newData.Value = EditorGUILayout.TextField(newData.Value);
                if (GUILayout.Button("Add"))
                {
                    bool error = false;
                    if (string.IsNullOrEmpty(newData.Key))
                    {
                        Debug.LogError("Add new data is fail, because key is invalid");
                        error = true;
                    }

                    if (!error && string.IsNullOrEmpty(newData.Value))
                    {
                        Debug.LogError("Add new data is fail, because value is invalid");
                        error = true;
                    }

                    if (!error && storage.HasKey(newData.Key))
                    {
                        Debug.LogError($"Add new data is fail, because key {newData.Key} is already exist");
                        error = true;
                    }

                    if (!error)
                    {
                        storage.SetData(newData.Key, newData.Value);
                        dataList.Add(newData);
                        newData = new PersistentData();
                        showNewDataBox = false;
                    }
                }

                if (GUILayout.Button("Delete"))
                {
                    showNewDataBox = false;
                }
            }
        }

        private void Delete()
        {
            PersistentManager.Delete(storage.Name);
            storage = PersistentManager.Load(storageName);
            showNewDataBox = false;
            RefreshData();
        }

        private class PersistentData
        {
            public string Key;
            public string Value;
        }
    }
}
