using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class DataTableManager : Singleton<DataTableManager>
    {
        private readonly Dictionary<Type, DataTableCollection> tableCollections = new Dictionary<Type, DataTableCollection>();

        public static void PreloadTable(Type tableType, Action callback = null)
        {
            Instance?.GetDataTableCollection(tableType).PreloadTable(callback);
        }

        public static void ReloadTableAsync(Type tableType, Action callback = null)
        {
            Instance?.GetDataTableCollection(tableType).ReloadTableAsync(callback);
        }

        public static void PreloadTable<T>(Action callback = null) where T : IDataTable
        {
            Instance?.GetDataTableCollection<T>().PreloadTable(callback);
        }

        public static void ReloadTable<T>() where T : IDataTable
        {
            Instance?.GetDataTableCollection<T>().ReloadTable();
        }

        public static void ReloadTableAsync<T>(Action callback = null) where T : IDataTable
        {
            Instance?.GetDataTableCollection<T>().ReloadTableAsync(callback);
        }

        public static bool HasTable<T>(string id) where T : class, IDataTable, new()
        {
            return Instance?.GetDataTableCollection<T>().GetTable<T>(id) != null;
        }

        public static string[] GetAllKeys<T>() where T : class, IDataTable, new()
        {
            return Instance?.GetDataTableCollection<T>().GetAllKeys();
        }

        public static void GetAllKeysAsync<T>(Action<string[]> callback) where T : class, IDataTable, new()
        {
            Instance?.GetDataTableCollection<T>().GetAllKeysAsync(callback);
        }

        public static void GetAllKeys<T>(List<string> result) where T : class, IDataTable, new()
        {
            Instance?.GetDataTableCollection<T>().GetAllKeys(result);
        }

        public static void GetAllKeysAsync<T>(List<string> result, Action<List<string>> callback) where T : class, IDataTable, new()
        {
            Instance?.GetDataTableCollection<T>().GetAllKeysAsync(result, callback);
        }

        public static T GetTable<T>(string id) where T : class, IDataTable, new()
        {
            return Instance?.GetDataTableCollection<T>().GetTable<T>(id);
        }

        public static void GetTableAsync<T>(string id, Action<T> callback) where T : class, IDataTable, new()
        {
            Instance?.GetDataTableCollection<T>().GetTableAsync(id, callback);
        }

        public static void UnloadAllTables()
        {
            Instance?.UnloadAllTablesInternal();
        }

        private void UnloadAllTablesInternal()
        {
            foreach (DataTableCollection collection in tableCollections.Values) collection.UnloadTable();

            tableCollections.Clear();
        }

        private DataTableCollection GetDataTableCollection<T>() where T : IDataTable
        {
            return GetDataTableCollection(typeof(T));
        }

        private DataTableCollection GetDataTableCollection(Type key)
        {
            if (!tableCollections.TryGetValue(key, out DataTableCollection collection))
            {
                collection = new DataTableCollection(key);
                tableCollections.Add(key, collection);
            }

            return collection;
        }
    }
}
