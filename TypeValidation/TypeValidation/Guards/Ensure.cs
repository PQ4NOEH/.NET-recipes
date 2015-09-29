using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeValidation.Guards
{
    public static class Ensure
    {
        public static void TheStringIsNotNullOrWhiteString(string str, string message)
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(message);
        }
        public static void IsInRange<T>(T item, T minimum, T maximum, string message) where T : IComparable
        {
            if (item.CompareTo(minimum) < 0) throw new ArgumentOutOfRangeException(message);
            else if (item.CompareTo(maximum) > 0) throw new ArgumentOutOfRangeException(message);
        }

        public static void MoreThanNYears(DateTime dateToValidate, UInt32 years, string message)
        {

            if (dateToValidate == default(DateTime)) throw new ArgumentOutOfRangeException(message);

            DateTime zeroTime = new DateTime(1, 1, 1);
            var currentTime = DateTime.Now;
            if (currentTime <= dateToValidate) throw new ArgumentOutOfRangeException(message);
            TimeSpan span = currentTime - dateToValidate;
            if (((zeroTime + span).Year - years) < years) throw new ArgumentOutOfRangeException(message);
        }
    }
}
