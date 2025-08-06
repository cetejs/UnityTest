using DataStructure;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class StackTest
    {
        [Test]
        public void Test()
        {
            Stack<int> stack = new Stack<int>();
            Debug.Log("Push(1, 2, 3, 4)");
            for (int i = 1; i < 5; i++)
            {
                stack.Push(i);
            }

            Debug.Log("PrePop(4, 3)");
            for (int i = 0; i < 2; i++)
            {
                Debug.Log("Pop: " + stack.Pop());
            }

            Debug.Log("PrePeek(2)");
            Debug.Log("Peek: " + stack.Peek());
            Debug.Log("Push(10, 11, 12)");
            for (int i = 10; i < 13; i++)
            {
                stack.Push(i);
            }
            Debug.Log("PrePeek(12)");
            Debug.Log("Peek: " + stack.Peek());
            Debug.Log("PreClear(5) Count: " + stack.Count);
            stack.Clear();
            Debug.Log("Count: " + stack.Count);
        }
    }
}