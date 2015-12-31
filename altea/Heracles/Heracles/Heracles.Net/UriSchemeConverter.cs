namespace Heracles.Net
{
    public static class UriSchemeConverter
    {
        public static UriScheme Convert(string scheme)
        {
            string normalizedScheme = scheme.Trim().ToUpperInvariant();

            switch (normalizedScheme)
            {
                case "HTTP":
                    return UriScheme.Http;

                case "HTTPS":
                    return UriScheme.HttpSecure;

                default:
                    return UriScheme.NotSupported;
            }
        }

        public static string Parse(UriScheme scheme)
        {
            switch (scheme)
            {
                case UriScheme.Http:
                    return "http";

                case UriScheme.HttpSecure:
                    return "https";

                default:
                    return null;
            }
        }
    }
}
