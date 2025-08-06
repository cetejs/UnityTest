namespace GameFramework
{
    public interface IGenericData
    {
    }

    public struct GenericData<T> : IGenericData
    {
        public T Item;

        public override string ToString()
        {
            return $"item = {Item}";
        }
    }

    public class GenericData<T1, T2> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}";
        }
    }

    public class GenericData<T1, T2, T3> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}";
        }
    }

    public class GenericData<T1, T2, T3, T4> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}";
        }
    }

    public class GenericData<T1, T2, T3, T4, T5> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}";
        }
    }

    public class GenericData<T1, T2, T3, T4, T5, T6> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;

        public override string ToString()
        {
            return
                $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}";
        }
    }

    public class GenericData<T1, T2, T3, T4, T5, T6, T7> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;

        public override string ToString()
        {
            return
                $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}";
        }
    }

    public class GenericData<T1, T2, T3, T4, T5, T6, T7, T8> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;

        public override string ToString()
        {
            return
                $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}, item8 = {Item8}";
        }
    }

    public class GenericData<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IGenericData
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;

        public override string ToString()
        {
            return
                $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}, item8 = {Item8}, item9 = {Item9}";
        }
    }
}
