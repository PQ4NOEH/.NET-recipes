namespace MicrosoftTranslator
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;

    public class MicrosoftSpeak : MicrosoftTranslator
    {
        private const string SPEAK_URI = BASE_URI + "Speak?appId=&text={0}&language={1}&format={2}&options={3}";

        public string Language { get; set; }
        public StreamFormat Format { get; set; }
        public StreamQuality Quality { get; set; }

        public MicrosoftSpeak(string clientId, string clientSecret, string language) :
            base(clientId, clientSecret)
        {
            this.Language = language;
            this.Format = StreamFormat.WAV;
            this.Quality = StreamQuality.MaxQuality;
        }

        public MicrosoftSpeak(string clientId, string clientSecret) :
            this(clientId, clientSecret, CultureInfo.CurrentCulture.Name.ToLower())
        {
        }

        public Stream GetStream(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length > MicrosoftSpeak.MAX_TEXT_LENGTH)
            {
                throw new ArgumentException("Invalid text");
            }

            string audioFormat = this.Format == StreamFormat.WAV ? "audio/wav" : "audio/mp3";
            string audioQuality = this.Quality == StreamQuality.MaxQuality ? "MaxQuality" : "MinSize";
            string speakUri = string.Format(
                MicrosoftSpeak.SPEAK_URI,
                Uri.EscapeDataString(text),
                this.Language,
                Uri.EscapeDataString(audioFormat),
                audioQuality);

            this.UpdateToken();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(speakUri);
            request.Headers.Add("Authorization", "Bearer " + this.accessToken);

            MemoryStream ms = new MemoryStream();

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            {
                responseStream.CopyTo(ms);
                ms.Position = 0;
            }

            return ms;
        }

        public byte[] GetBytes(string text)
        {
            byte[] bytes;
            using (MemoryStream stream = (MemoryStream)this.GetStream(text))
            {
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}