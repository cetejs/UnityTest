using System;
using System.Collections;

namespace GameFramework
{
    public partial class GenericObjectPool
    {
        private class GenericObjectCollection
        {
            private Type type;
            private bool isRecyclable;
            private Stack objects;

            public GenericObjectCollection(Type type)
            {
                this.type = type;
                isRecyclable = typeof(IRecyclable).IsAssignableFrom(type);
                objects = new Stack();
            }

            public T Get<T>()
            {
                lock (objects)
                {
                    if (objects.Count > 0)
                    {
                        return (T)objects.Pop();
                    }
                }

                if (!type.IsArray)
                {
                    return Activator.CreateInstance<T>();
                }

                return (T)Activator.CreateInstance(type, 0);
            }

            public void Release(object obj)
            {
                if (isRecyclable)
                {
                    ((IRecyclable)obj).Clear();
                }

                lock (objects)
                {
                    objects.Push(obj);
                }
            }

            public void Add(int count)
            {
                lock (objects)
                {
                    while (count-- > 0)
                    {
                        if (!type.IsArray)
                        {
                            objects.Push(Activator.CreateInstance(type));
                        }
                        else
                        {
                            objects.Push(Activator.CreateInstance(type, 0));
                        }
                    }
                }
            }

            public void Clear()
            {
                lock (objects)
                {
                    objects.Clear();
                }
            }
        }
    }
}
