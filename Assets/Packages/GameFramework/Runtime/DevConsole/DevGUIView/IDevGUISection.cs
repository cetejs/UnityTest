using System;

namespace GameFramework
{
    public interface IDevGUISection
    {
        void AddSelector<T>(string name, Func<T> get, Action<T> set, float step = 1f);

        void RemoveSelector(string name);

        void AddMethod(string name, Action method);

        void AddMethod<T>(string name, Action<T> method);

        void AddMethod<T1, T2>(string name, Action<T1, T2> method);

        void AddMethod<T1, T2, T3>(string name, Action<T1, T2, T3> method);

        void AddMethod<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> method);

        void AddMethod<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> method);

        void AddMethod<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> method);

        void AddMethod<T1, T2, T3, T4, T5, T6, T7>(string name, Action<T1, T2, T3, T4, T5, T6, T7> method);

        void AddMethod<T1, T2, T3, T4, T5, T6, T7, T8>(string name, Action<T1, T2, T3, T4, T5, T6, T7, T8> method);

        void AddMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string name, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> method);

        void RemoveMethod(string name);
    }
}
