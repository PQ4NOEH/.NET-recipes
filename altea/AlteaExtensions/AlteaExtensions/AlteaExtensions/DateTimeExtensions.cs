
namespace Altea.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToTimestamp(this DateTime @this)
        {
            return Convert.ToInt64((@this - UnixEpoch).TotalMilliseconds);
        }

        public static DateTime TimestampToDateTime(this long @this)
        {
            return UnixEpoch.AddMilliseconds(@this);
        }
    }
}
