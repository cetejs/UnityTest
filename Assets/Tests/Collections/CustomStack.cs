using System;

namespace Tests
{
    public class CustomStack<T>
    {
        private T[] items;
        private int size;

        private static readonly T[] EmptyArray = Array.Empty<T>();

        public int Count => size;

        public CustomStack()
        {
            items = EmptyArray;
            size = 0;
        }

        public CustomStack(int capacity)
        {
            items = new T[capacity];
            size = 0;
        }

        public void Push(T item)
        {
            if (size == items.Length)
            {
                Resize();
            }

            items[size++] = item;
        }

        public T Pop()
        {
            if (size == 0)
            {
                throw new ArgumentOutOfRangeException(null, "Stack is empty.");
            }

            return items[--size];
        }

        public T Peek()
        {
            if (size == 0)
            {
                throw new ArgumentOutOfRangeException(null, "Stack is empty.");
            }

            return items[size - 1];
        }

        public void Clear()
        {
            size = 0;
        }

        private void Resize()
        {
            if (items.Length == 0)
            {
                items = new T[2];
            }
            else
            {
                T[] newItems = new T[items.Length * 2];
                for (int i = 0; i < items.Length; i++)
                {
                    newItems[i] = items[i];
                }

                items = newItems;
            }
        }
    }
}