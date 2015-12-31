namespace Altea.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string CamelCaseToSnakeCase(this string @this)
        {
            string snakeCase = Regex.Replace(@this, @"([A-Z]+)([A-Z][a-z])", "$1_$2");
            snakeCase = Regex.Replace(snakeCase, @"([a-z\d])([A-Z])", "$1_$2");
            snakeCase = snakeCase.Replace("-", "_");
            snakeCase = snakeCase.Replace(" ", "_");
            return snakeCase.ToLowerInvariant();
        }
    }
}
