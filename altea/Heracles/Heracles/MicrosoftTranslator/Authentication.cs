namespace MicrosoftTranslator
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;

    internal class Authentication
    {
        private const string DATAMARKET_ACCESS_URI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private const string TRANSLATOR_SCOPE_URI = "http://api.microsofttranslator.com";

        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;

        public Authentication(string clientId, string clientSecret)
        {
            this.CLIENT_ID = Uri.EscapeDataString(clientId);
            this.CLIENT_SECRET = Uri.EscapeDataString(clientSecret);
        }

        private string GenerateRequestParameters()
        {
            return "grant_type=client_credentials"
                + "&client_id=" + this.CLIENT_ID
                + "&client_secret=" + this.CLIENT_SECRET
                + "&scope=" + TRANSLATOR_SCOPE_URI;
        }

        public AuthToken GetToken()
        {
            WebRequest request = WebRequest.Create(DATAMARKET_ACCESS_URI);
            byte[] parameters = System.Text.Encoding.ASCII.GetBytes(this.GenerateRequestParameters());

            request.Method = "POST";
            request.ContentLength = parameters.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(parameters, 0, parameters.Length);
            }

            AuthToken token;

            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AuthToken));

                // ReSharper disable once AssignNullToNotNullAttribute
                token = (AuthToken)serializer.ReadObject(stream);
            }

            return token;
        }
    }
}
