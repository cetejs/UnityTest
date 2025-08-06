using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class GenericObjectPool : Singleton<GenericObjectPool>
    {
        private Dictionary<Type, GenericObjectCollection> collections = new Dictionary<Type, GenericObjectCollection>();

        public static T Get<T>() where T : class
        {
            return Instance?.GetCollection<T>().Get<T>();
        }

        public static void Release<T>(T obj) where T : class
        {
            Instance?.GetCollection<T>().Release(obj);
        }

        public static void Add<T>(int count) where T : class
        {
            Instance?.GetCollection<T>().Add(count);
        }

        public static void Clear<T>() where T : class
        {
            Instance?.GetCollection<T>().Clear();
        }

        public static void Clear()
        {
            Instance?.collections.Clear();
        }

        private GenericObjectCollection GetCollection<T>()
        {
            Type type = typeof(T);
            GenericObjectCollection collection;
            lock (collections)
            {
                if (!collections.TryGetValue(type, out collection))
                {
                    collection = new GenericObjectCollection(type);
                    collections.Add(type, collection);
                }
            }

            return collection;
        }
    }
}
