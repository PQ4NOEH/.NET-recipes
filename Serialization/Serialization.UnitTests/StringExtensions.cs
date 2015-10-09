using System;
using System.Linq;

namespace Serialization.UnitTests
{
    public static class StringExtensions
    {
        public static string RandomString( int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var charArray = (length)
                                .Times<char>(new Func<char>(() => chars[random.Next(chars.Length)]))
                                .ToArray();
            return new String(charArray);
        }
    }
}
