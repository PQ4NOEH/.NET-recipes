using System;

namespace TheMenu.Core.Guards
{
    public class NotNullOrWhiteSpaceString
    {
        readonly string _value;
        public NotNullOrWhiteSpaceString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("The value can not be null");
            _value = value;
        }

        public static implicit operator NotNullOrWhiteSpaceString(string value)
        {
            return new NotNullOrWhiteSpaceString(value);
        }

        public static implicit operator string(NotNullOrWhiteSpaceString str)
        {
            return str.Value;
        }

        public string Value { get { return _value; } }
    }
}
