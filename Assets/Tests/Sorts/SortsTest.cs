using System;
using UnityEngine;

namespace Tests
{
    public class SortsTest : MonoBehaviour
    {
        private void Start()
        {
            SortArray(Sorts.SelectionSort, "SelectionSort");
            SortArray(Sorts.BubbleSort, "BubbleSort");
            SortArray(Sorts.QuickSort, "QuickSort");
            SortArray(Sorts.InsertSort, "InsertSort");
            SortArray(Sorts.ShellSort, "ShellSort");
            SortArray(Sorts.MergeSort, "MergeSort");
            SortArray(Sorts.HeapSort, "HeapSort");
            SortArray(Sorts.CountingSort, "CountingSort");
            SortArray(Sorts.BucketSort, "BucketSort");
            SortArray(Sorts.RadixSort, "RadixSort");
        }

        private void SortArray(Action<int[]> sort, string name)
        {
            int[] array = { 2, 50, 61, 71, 7, 23, 18, 1, 67, 2, 3, 50, 7, 23, 65, 74, 999, 27, 68, 21};
            sort.Invoke(array);
            Debug.Log(string.Join(", ", array) + " - " + name);
        }
    }
}
