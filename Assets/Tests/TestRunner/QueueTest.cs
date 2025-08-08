using DataStructure;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class QueueTest
    {
        [Test]
        public void Test()
        {
            CustomQueue<int> queue = new CustomQueue<int>();
            Debug.Log("Enqueue(1, 2, 3, 4)");
            for (int i = 1; i < 5; i++)
            {
                queue.Enqueue(i);
            }

            Debug.Log("PreDequeue(1, 2)");
            for (int i = 0; i < 2; i++)
            {
                Debug.Log("Dequeue: " + queue.Dequeue());
            }

            Debug.Log("PrePeek(3)");
            Debug.Log("Peek: " + queue.Peek());
            Debug.Log("Enqueue(10, 11, 12)");
            for (int i = 10; i < 13; i++)
            {
                queue.Enqueue(i);
            }
            Debug.Log("PrePeek(3)");
            Debug.Log("Peek: " + queue.Peek());
            Debug.Log("PreClear(5) Count: " + queue.Count);
            queue.Clear();
            Debug.Log("Count: " + queue.Count);
        }
    }
}