using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public partial class UnityCoroutine : PersistentSingleton<UnityCoroutine>
    {
        private Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();

        private Coroutine BeginInternal(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        private Coroutine BeginInternal(string hash, IEnumerator routine)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Please execute in run mode");
                return null;
            }

            StopInternal(hash);
            Coroutine coroutine = StartCoroutine(routine);
            coroutines[hash] = coroutine;
            return coroutine;
        }

        private void StopInternal(IEnumerator routine)
        {
            StopCoroutine(routine);
        }

        private void StopInternal(Coroutine routine)
        {
            StopCoroutine(routine);
        }

        private void StopInternal(string hash)
        {
            if (coroutines.TryGetValue(hash, out Coroutine coroutine))
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                coroutines.Remove(hash);
            }
        }

        private void StopAllInternal()
        {
            StopAllCoroutines();
            coroutines.Clear();
        }
    }
}
