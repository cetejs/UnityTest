using System;

namespace Tests
{
    public class CustomQueue<T>
    {
        private T[] items;
        private int head;
        private int tail;
        private int size;

        private static readonly T[] EmptyArray = Array.Empty<T>();

        public int Count => size;

        public CustomQueue()
        {
            items = EmptyArray;
            head = 0;
            tail = 0;
            size = 0;
        }

        public CustomQueue(int capacity)
        {
            items = new T[capacity];
            head = 0;
            tail = 0;
            size = 0;
        }

        public void Enqueue(T item)
        {
            if (size == items.Length)
            {
                Resize();
            }

            items[tail] = item;
            tail = (tail + 1) % items.Length;
            size++;
        }

        public T Dequeue()
        {
            if (size == 0)
            {
                throw new ArgumentOutOfRangeException(null, "Queue is empty.");
            }

            var item = items[head];
            head = (head + 1) % items.Length;
            size--;
            return item;
        }

        public T Peek()
        {
            if (size == 0)
            {
                throw new ArgumentOutOfRangeException(null, "Queue is empty.");
            }

            return items[head];
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
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
                int index = 0;
                if (head < tail)
                {
                    for (int i = head; i < tail; i++)
                    {
                        newItems[index++] = items[i];
                    }
                }
                else
                {
                    for (int i = head; i < items.Length; i++)
                    {
                        newItems[index++] = items[i];
                    }
                    
                    for (int i = 0; i < tail; i++)
                    {
                        newItems[index++] = items[i];
                    }
                }

                head = 0;
                tail = index;
                size = index;
                items = newItems;
            }
        }
    }
}