using System;
using System.Collections.Generic;

namespace Algorithm
{
    public static class Sorts
    {
        public static void SelectionSort<T>(T[] array) where T : IComparable<T>
        {
            for (int i = 0; i < array.Length; i++)
            {
                int min = i;
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[min].CompareTo(array[j]) > 0)
                    {
                        min = j;
                    }
                }

                T temp = array[i];
                array[i] = array[min];
                array[min] = temp;
            }
        }

        public static void BubbleSort<T>(T[] array) where T : IComparable<T>
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 0; j < array.Length - i - 1; j++)
                {
                    if (array[j].CompareTo(array[j + 1]) > 0)
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        public static void QuickSort<T>(T[] array) where T : IComparable<T>
        {
            QuickSort(array, 0, array.Length - 1);
        }

        private static void QuickSort<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            if (low >= high)
            {
                return;
            }

            int mid = QuickPart(array, low, high);
            QuickSort(array, low, mid - 1);
            QuickSort(array, mid + 1, high);
        }

        private static int QuickPart<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            T key = array[low];
            while (low < high)
            {
                while (low < high && key.CompareTo(array[high]) <= 0)
                {
                    high--;
                }

                if (low < high)
                {
                    array[low++] = array[high];
                }

                while (low < high && key.CompareTo(array[low]) >= 0)
                {
                    low++;
                }

                if (low < high)
                {
                    array[high--] = array[low];
                }
            }

            array[low] = key;
            return low;
        }

        public static void InsertSort<T>(T[] array) where T : IComparable<T>
        {
            for (int i = 1; i < array.Length; i++)
            {
                T key = array[i];
                int j = i;
                while (j > 0 && array[j - 1].CompareTo(key) > 0)
                {
                    array[j] = array[j - 1];
                    j--;
                }

                array[j] = key;
            }
        }

        public static void ShellSort<T>(T[] array) where T : IComparable<T>
        {
            for (int gap = array.Length / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < array.Length; i++)
                {
                    T key = array[i];
                    int j = i;
                    while (j >= gap && array[j - gap].CompareTo(key) > 0)
                    {
                        array[j] = array[j - gap];
                        j -= gap;
                    }

                    array[j] = key;
                }
            }
        }

        public static void MergeSort<T>(T[] array) where T : IComparable<T>
        {
            MergeSort(array, 0, array.Length - 1);
        }
        
        private static void MergeSort<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            if (low >= high)
            {
                return;
            }

            int mid = (low + high) / 2;
            MergeSort(array, low, mid);
            MergeSort(array,  mid + 1, high);
            Merge(array, low, mid, high);
        }

        private static void Merge<T>(T[] array, int low, int mid, int high) where T : IComparable<T>
        {
            int length = high - low + 1;
            T[] temp = new T[length];
            Array.Copy(array, low, temp, 0, length);
            int i = low, j = mid + 1;
            for (int k = low; k <= high; k++)
            {
                if (i > mid)
                {
                    array[k] = temp[j - low];
                    j++;
                }
                else if (j > high)
                {
                    array[k] = temp[i - low];
                    i++;
                }
                else if (temp[i - low].CompareTo(temp[j - low]) <= 0)
                {
                    array[k] = temp[i - low];
                    i++;
                }
                else
                {
                    array[k] = temp[j - low];
                    j++;
                }
            }
        }

        public static void HeapSort<T>(T[] array) where T : IComparable<T>
        {
            for (int i = array.Length / 2; i >= 0; i--)
            {
                Heapify(array, i, array.Length);
            }

            for (int i = array.Length - 1; i >= 0; i--)
            {
                T temp = array[0];
                array[0] = array[i];
                array[i] = temp;
                Heapify(array, 0, i);
            }
        }

        private static void Heapify<T>(T[] array, int index, int count) where T : IComparable<T>
        {
            int i = index;
            int l = index * 2 + 1;
            int r = index * 2 + 2;

            if (l < count && array[l].CompareTo(array[i]) > 0)
            {
                i = l;
            }

            if (r < count && array[r].CompareTo(array[i]) > 0)
            {
                i = r;
            }

            if (i != index)
            {
                T temp = array[i];
                array[i] = array[index];
                array[index] = temp;
                Heapify(array, i, count);
            }
        }

        public static void CountingSort(int[] array)
        {
            int min = array[0];
            int max = array[0];

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                }
                else if (array[i] > max)
                {
                    max = array[i];
                }
            }

            int[] count = new int[max - min + 1];
            for (int i = 0; i < array.Length; i++)
            {
                count[array[i] - min]++;
            }

            int n = 0;
            for (int i = 0; i < count.Length; i++)
            {
                for (int j = 0; j < count[i]; j++)
                {
                    array[n++] = i + min;
                }
            }
        }

        public static void BucketSort(int[] array)
        {
            int min = array[0];
            int max = array[0];

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                }
                else if (array[i] > max)
                {
                    max = array[i];
                }
            }

            int count = (max - min) / array.Length + 1;
            List<int>[] buckets = new List<int>[count];
            for (int i = 0; i < array.Length; i++)
            {
                int index = (array[i] - min) / array.Length;
                if (buckets[index] == null)
                {
                    buckets[index] = new List<int>();
                }

                buckets[index].Add(array[i]);
            }

            for (int i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] != null)
                {
                    buckets[i].Sort();
                }
            }

            int n = 0;
            for (int i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] != null)
                {
                    for (int j = 0; j < buckets[i].Count; j++)
                    {
                        array[n++] = buckets[i][j];
                    }
                }
            }
        }

        public static void RadixSort(int[] array)
        {
            int max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                }
            }

            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                CountingSort(array, exp);
            }
        }

        private static void CountingSort(int[] array, int exp)
        {
            int[] count = new int[10];
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                count[array[i] / exp % 10]++;
            }

            for (int i = 1; i < count.Length; i++)
            {
                count[i] += count[i - 1];
            }

            for (int i = array.Length - 1; i >= 0; i--)
            {
                int index = --count[array[i] / exp % 10];
                output[index] = array[i];
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = output[i];
            }
        }
    }
}
