using System;

namespace AlteaLabs.Core.Guards
{
    public class NotNulllEmptyOrWhiteSpaceString
    {
        private readonly string _value;
        public NotNulllEmptyOrWhiteSpaceString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException("string can not be null empty or white string");
            _value = value;
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public static implicit operator NotNulllEmptyOrWhiteSpaceString(string value)
        {
            return new NotNulllEmptyOrWhiteSpaceString(value);
        }

        public static implicit operator string(NotNulllEmptyOrWhiteSpaceString obj)
        {
            return obj.Value;
        }
    }
}
