using System;
using System.Collections.Generic;

namespace Serialization.UnitTests
{
    public static class NumberExtensions
    {
        public static void Times(this int i, Action action)
        {
            for (int count = 0; count < i; count++) action();
        }
        public static void Times(this int i, Action<int> action)
        {
            for (int count = 0; count < i; count++) action(count);
        }
        public static IEnumerable<T> Times<T>(this int i, Func<T> func)
        {
            List<T> result = new List<T>();
            for (int count = 0; count < i; count++) result.Add(func());
            return result;
        }
        public static IEnumerable<T> Times<T>(this int i, Func<int, T> func)
        {
            List<T> result = new List<T>();
            for (int count = 0; count < i; count++) result.Add(func(count));
            return result;
        }
    }
}
