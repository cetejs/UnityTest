using UnityEngine;

namespace GameFramework
{
    public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        protected static T Instance
        {
            get { return SingletonCreator.GetMonoSingleton<T>(true); }
        }
    }
}
