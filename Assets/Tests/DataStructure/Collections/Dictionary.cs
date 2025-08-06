using System;

namespace DataStructure
{
    public class Dictionary<TKey, TValue>
    {
        private struct Entry
        {
            public int HashCode;
            public TKey Key;
            public TValue Value;
            public int Next;
        }

        private Entry[] entries;
        private int[] buckets;
        private int count;
        private int freeList;
        private int freeCount;

        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                {
                    return entries[i].Value;
                }

                throw new IndexOutOfRangeException();
            }
            set => Insert(key, value, false);
        }

        public int Count => count - freeCount;

        public Dictionary() : this(2)
        {
        }

        public Dictionary(int capacity)
        {
            entries = new Entry[capacity];
            buckets = new int[capacity];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }

            count = 0;
            freeList = -1;
            freeCount = 0;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool RemoveKey(TKey key)
        {
            int hashCode = key.GetHashCode() & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;
            int last = -1;
            for (int i = buckets[targetBucket]; i >= 0; last = i, i = entries[i].Next)
            {
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                {
                    if (last >= 0)
                    {
                        entries[last].Next = entries[i].Next;
                    }
                    else
                    {
                        buckets[targetBucket] = entries[i].Next;
                    }

                    entries[i].HashCode = -1;
                    entries[i].Key = default;
                    entries[i].Value = default;
                    entries[i].Next = freeList;
                    freeList = i;
                    freeCount++;
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public void Clear()
        {
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i] = default;
            }

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }

            count = 0;
            freeList = -1;
            freeCount = 0;
        }

        private int FindEntry(TKey key)
        {
            int hashCode = key.GetHashCode() & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].Next)
            {
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            int hashCode = key.GetHashCode() & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].Next)
            {
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                {
                    if (add)
                    {
                        throw new ArgumentException("An item with the same key has already been added.");
                    }

                    entries[i].Value = value;
                    return;
                }
            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].Next;
                freeCount--;
            }
            else
            {
                if (entries.Length == count)
                {
                    Resize();
                    targetBucket = hashCode % buckets.Length;
                }

                index = count;
                count++;
            }

            entries[index].HashCode = hashCode;
            entries[index].Key = key;
            entries[index].Value = value;
            entries[index].Next = buckets[targetBucket];
            buckets[targetBucket] = index;
        }

        private void Resize()
        {
            int newSize = count * 2;
            Entry[] newEntries = new Entry[newSize];
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++)
            {
                newBuckets[i] = -1;
            }

            for (int i = 0; i < count; i++)
            {
                newEntries[i] = entries[i];
                if (newEntries[i].HashCode > 0)
                {
                    int bucket = newEntries[i].HashCode % newBuckets.Length;
                    newEntries[i].Next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            entries = newEntries;
            buckets = newBuckets;
        }
    }
}