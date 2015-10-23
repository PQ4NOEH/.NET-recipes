using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMenu.Core.Guards
{
    public class NotNullable<T>
    {
        readonly T _value;
        public NotNullable(T value)
        {
            if (value == null) throw new ArgumentNullException("The value can not be null");
            _value = value;
        }

        public static implicit operator NotNullable<T>(T value)
        {
            return new NotNullable<T>(value);
        }

        public static implicit operator T(NotNullable<T> notNullable)
        {
            return notNullable.Value;
        }

        public T Value { get { return _value; } }
    }
}
