using System;

namespace AlteaLabs.Common.Guards
{
    public class NotNullable<T>
    {
        private readonly T _objReference;
        public NotNullable(T obj)
        {
            if (obj == null) throw  new ArgumentNullException();
            _objReference = obj;
        }

        public T WrappedObject
        {
            get
            {
                return _objReference;
            }
        }

        public static implicit operator NotNullable<T>(T obj)
        {
            return new NotNullable<T>(obj);
        }

        public static implicit operator T( NotNullable<T> obj)
        {
            return obj.WrappedObject;
        }
    }
}
