namespace Altea.Extensions
{
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;

    public static class HttpRequestBaseExtensions
    {
        public static IPAddress GetIpAddress(this HttpRequestBase @this)
        {
            if (@this == null)
            {
                return null;
            }

            NameValueCollection serverVariables = @this.ServerVariables;
            string forwardedIps = serverVariables["HTTP_X_FORWARDED_FOR"];
            string ip = !string.IsNullOrEmpty(forwardedIps) ? forwardedIps.Split(',')[0] : serverVariables["REMOTE_ADDR"];

            IPAddress address;
            return IPAddress.TryParse(ip, out address) ? address : null;
        }
    }
}
