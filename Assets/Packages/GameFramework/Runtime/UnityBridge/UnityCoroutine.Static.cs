using System.Collections;
using UnityEngine;

namespace GameFramework
{
    public partial class UnityCoroutine
    {
        public static Coroutine Begin(IEnumerator routine)
        {
            return Instance?.BeginInternal(routine);
        }

        public static Coroutine Begin(string hash, IEnumerator routine)
        {
            return Instance?.BeginInternal(hash, routine);
        }

        public static void Stop(IEnumerator routine)
        {
            Instance?.StopInternal(routine);
        }

        public static void Stop(Coroutine routine)
        {
            Instance?.StopInternal(routine);
        }

        public static void Stop(string hash)
        {
            Instance?.StopInternal(hash);
        }

        public static void StopAll()
        {
            Instance?.StopAllInternal();
        }
    }
}
