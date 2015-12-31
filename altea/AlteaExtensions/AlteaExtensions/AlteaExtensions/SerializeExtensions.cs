namespace Altea.Extensions
{
    using Newtonsoft.Json;

    public static class SerializeExtensions
    {
        public static string ToJson<T>(this T @this)
        {
            return JsonConvert.SerializeObject(@this);
        }

        public static T FromJson<T>(this string @this)
        {
            return JsonConvert.DeserializeObject<T>(@this);
        }
    }
}
