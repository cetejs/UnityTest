using DataStructure;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ListTest
    {
        [Test]
        public void Test()
        {
            CustomList<int> list = new CustomList<int>();
            Debug.Log("Add(1, 2, 3, 4)");
            for (int i = 1; i < 5; i++)
            {
                list.Add(i);
            }

            Debug.Log("Remove(1, 4)");
            list.Remove(1);
            list.Remove(4);
            
            Debug.Log("Insert(0) 6");
            list.Insert(0, 6);
            Debug.Log("PreGet(0) 6");
            Debug.Log("Get(0): " + list[0]);
            Debug.Log("AddRange(10, 11, 12)");
            Debug.Log("InsetAddRange(3, [13, 14, 15])");
            int[] range1 = { 10, 11, 12 };
            int[] range2 = { 13, 14, 15 };
            list.AddRange(range1);
            list.InsertRange(3, range2);
            Debug.Log("For[6, 2, 3, 13, 14, 15 ,10, 11, 12]");
            string output = "";
            for (int i = 0; i < list.Count; i++)
            {
                output += list[i] + ", ";
            }
            Debug.Log(output);
            Debug.Log("PreClear() Count: " + list.Count);
            list.Clear();
            Debug.Log("Count: " + list.Count);
        }
    }
}