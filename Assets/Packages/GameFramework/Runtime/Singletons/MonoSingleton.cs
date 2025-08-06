using UnityEngine;

namespace GameFramework
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T Instance
        {
            get { return SingletonCreator.GetMonoSingleton<T>(false); }
        }
    }
}
