namespace Altea.Extensions
{
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static bool TryGetTypedValue<TKey, TValue, TActual>(
            this IDictionary<TKey, TValue> data,
            TKey key,
            out TActual value) where TActual : TValue
        {
            TValue tmp;
            if (data.TryGetValue(key, out tmp))
            {
                value = (TActual)tmp;
                return true;
            }

            value = default(TActual);
            return false;
        }
    }
}
