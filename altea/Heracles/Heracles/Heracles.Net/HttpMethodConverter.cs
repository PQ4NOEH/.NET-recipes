namespace Heracles.Net
{
    public static class HttpMethodConverter
    {
        public static HttpMethod Convert(string method)
        {
            string normalizedMethod = method == null ? null : method.Trim().ToUpperInvariant();

            switch (normalizedMethod)
            {
                case "GET":
                    return HttpMethod.Get;

                case "POST":
                    return HttpMethod.Post;

                default:
                    return HttpMethod.NotSupported;
            }
        }

        public static string Parse(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.Get:
                    return "GET";

                case HttpMethod.Post:
                    return "POST";

                default:
                    return null;
            }   
        }
    }
}
