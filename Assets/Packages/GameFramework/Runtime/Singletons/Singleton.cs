namespace GameFramework
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T Instance
        {
            get { return SingletonCreator.GetSingleton<T>(); }
        }
    }
}
