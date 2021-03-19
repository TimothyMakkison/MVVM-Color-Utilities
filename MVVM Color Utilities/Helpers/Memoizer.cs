using System;
using System.Collections.Concurrent;

namespace MVVM_Color_Utilities.Helpers
{
    public class Memoizer<T1, U>
    {
        private readonly Func<T1, U> func;
        private readonly ConcurrentDictionary<T1, U> dict = new ConcurrentDictionary<T1, U>();

        public Memoizer(Func<T1, U> func)
        {
            this.func = func;
        }

        public U GetValue(T1 param)
        {
            var val = dict.GetOrAdd(param, func);
            return val;
        }
    }
    public class Memoizer<T1, T2, U>
    {
        private readonly Func<(T1, T2), U> func;
        private readonly ConcurrentDictionary<(T1, T2), U> dict = new ConcurrentDictionary<(T1, T2), U>();

        public Memoizer(Func<T1, T2, U> func)
        {
            this.func = tup => func(tup.Item1, tup.Item2);
        }

        public U GetValue(T1 t1, T2 t2)
        {
            var val = dict.GetOrAdd((t1, t2), func);
            return val;
        }
    }
}