using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public partial class Scheduler : MonoSingleton<Scheduler>
    {
        private SortedLinkedList<DelegateInfo> delegateInfos = new SortedLinkedList<DelegateInfo>();
        private Queue<DelegateInfo>[] actions = { new Queue<DelegateInfo>(), new Queue<DelegateInfo>() };
        private int collectionIndex;

        public static int Schedule(Action action, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay);
        }

        public static int Schedule<T>(Action<T> action, T arg, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, arg);
        }

        public static int Schedule<T1, T2>(Action<T1, T2> action, T1 item1, T2 item2, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2);
        }

        public static int Schedule<T1, T2, T3>(Action<T1, T2, T3> action, T1 item1, T2 item2, T3 item3, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3);
        }

        public static int Schedule<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 item1, T2 item2, T3 item3, T4 item4, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4);
        }

        public static int Schedule<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4, item5);
        }

        public static int Schedule<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6);
        }

        public static int Schedule<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7);
        }

        public static int Schedule<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7, item8);
        }

        public static int Schedule<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, float delay = 0f)
        {
            return Instance == null ? 0 : Instance.InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7, item8, item9);
        }

        public static void Cancel(int id)
        {
            Instance?.InternalRemoveAction(id);
        }

        private int InternalAddAction(Delegate action, float delay = 0f, params object[] param)
        {
            DelegateInfo del = new DelegateInfo(action, Time.time + delay, param);
            if (delay > 0f)
            {
                delegateInfos.Add(del);
            }
            else
            {
                actions[collectionIndex].Enqueue(del);
            }

            return del.Id;
        }

        private void InternalRemoveAction(int id)
        {
            LinkedListNode<DelegateInfo> current = delegateInfos.First;
            while (current != null)
            {
                if (current.Value.Id == id)
                {
                    delegateInfos.Remove(current);
                    break;
                }

                current = current.Next;
            }
        }

        private void LateUpdate()
        {
            int iterationCount = 0;
            while (delegateInfos.Count > 0 && delegateInfos.First.Value.InvocationTime <= Time.unscaledTime)
            {
                actions[collectionIndex].Enqueue(delegateInfos.RemoveFirst());
            }

            do
            {
                int invokeIndex = collectionIndex;
                collectionIndex = (collectionIndex + 1) % 2;
                Queue<DelegateInfo> queue = actions[invokeIndex];
                while (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();
                }

                iterationCount++;
                Debug.Assert(iterationCount < 100);
            } while (actions[collectionIndex].Count > 0);
        }

        public override string ToString()
        {
            return string.Join("\n", delegateInfos);
        }
    }
}
