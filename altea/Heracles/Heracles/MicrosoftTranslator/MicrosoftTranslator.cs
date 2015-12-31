namespace MicrosoftTranslator
{
    using System;

    public abstract class MicrosoftTranslator
    {
        protected const string BASE_URI = "http://api.microsofttranslator.com/V2/Http.svc/";
        protected const int MAX_TEXT_LENGTH = 1000;
        protected const int MAX_AUTODETECT_TEXT_LENGTH = 100;

        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;

        protected string accessToken;

        protected MicrosoftTranslator(string clientId, string clientSecret)
        {
            this.CLIENT_ID = clientId;
            this.CLIENT_SECRET = clientSecret;
        }

        protected void UpdateToken()
        {
            if (string.IsNullOrEmpty(this.CLIENT_ID))
            {
                throw new ArgumentException("Invalid Client ID");
            }

            if (string.IsNullOrEmpty(this.CLIENT_SECRET))
            {
                throw new ArgumentException("Invalid Client Secret");
            }

            Authentication auth = new Authentication(this.CLIENT_ID, this.CLIENT_SECRET);
            AuthToken token = auth.GetToken();

            this.accessToken = token.AccessToken;
        }
    }
}
