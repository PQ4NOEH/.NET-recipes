using System;

namespace AlteaLabs.Core.Guards
{
    public class NotNullable<T>
    {
        private readonly T _value;
        public NotNullable(T obj)
        {
            if (obj == null) throw new ArgumentNullException();
            _value = obj;
        }

        public T Value
        {
            get
            {
                return _value;
            }
        }

        public static implicit operator NotNullable<T>(T obj)
        {
            return new NotNullable<T>(obj);
        }

        public static implicit operator T(NotNullable<T> obj)
        {
            return obj._value;
        }
    }
}
