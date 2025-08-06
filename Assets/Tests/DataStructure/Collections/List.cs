using System;

namespace DataStructure
{
    public class List<T>
    {
        private T[] items;
        private int size;

        private static readonly T[] EmptyArray = Array.Empty<T>();

        public int Count => size;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
                
                return items[index];
            }
            set
            {
                if (index < 0 || index >= size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }

                items[index] = value;
            }
        }

        public List()
        {
            items = EmptyArray;
            size = 0;
        }

        public List(int capacity)
        {
            items = new T[capacity];
            size = 0;
        }

        public void Add(T item)
        {
            Insert(size, item);
        }

        public bool Remove(T item)
        {
            int index = Indexof(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Contains(T item)
        {
            return Indexof(item) >= 0;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            if (size == items.Length)
            {
                Resize(size);
            }

            if (index < size)
            {
                for (int i = size; i > index; i--)
                {
                    items[i] = items[i - 1];
                }
            }

            items[index] = item;
            size++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            for (int i = index; i < size - 1; i++)
            {
                items[i] = items[i + 1];
            }

            size--;
        }

        public int Indexof(T item)
        {
            for (int i = 0; i < size; i++)
            {
                if (items[i].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddRange(List<T> items)
        {
            InsertRange(size, items);
        }

        public void AddRange(T[] items)
        {
            InsertRange(size, items);
        }

        public void InsertRange(int index, List<T> items)
        {
            if (items == null)
            {
                throw new NullReferenceException("Items is null.");
            }

            InsertRange(index, items.items);
        }

        public void InsertRange(int index, T[] items)
        {
            if (index < 0 || index > size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            if (items == null)
            {
                throw new NullReferenceException("Items is null.");
            }

            if (items.Length == 0)
            {
                return;
            }

            int newSize = size + items.Length;
            if (newSize > items.Length)
            {
                Resize(newSize);
            }

            if (index < size)
            {
                for (int i = newSize - 1; i >= size; i--)
                {
                    this.items[i] = this.items[i - items.Length];
                }
            }

            for (int i = index; i < index + items.Length; i++)
            {
                this.items[i] = items[i - index];
            }

            size = newSize;
        }

        public void Clear()
        {
            size = 0;
        }

        private void Resize(int newSize)
        {
            if (items.Length == 0)
            {
                if (newSize < 2)
                {
                    newSize = 2;
                }

                items = new T[newSize];
            }
            else
            {
                int length = items.Length * 2;
                if (newSize < length)
                {
                    newSize = length;
                }

                T[] newItems = new T[newSize];
                for (int i = 0; i < items.Length; i++)
                {
                    newItems[i] = items[i];
                }

                items = newItems;
            }
        }
    }
}