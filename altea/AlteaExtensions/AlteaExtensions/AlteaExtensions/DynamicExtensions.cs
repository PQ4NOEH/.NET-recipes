namespace Altea.Extensions
{
    public static class DynamicExtensions
    {
        public static T Cast<T>(this object @this)
        {
            return (T)@this;
        }
    }
}
