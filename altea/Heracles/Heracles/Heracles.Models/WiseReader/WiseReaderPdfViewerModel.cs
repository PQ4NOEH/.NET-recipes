namespace Heracles.Models.WiseReader
{
    using Newtonsoft.Json;

    public class WiseReaderPdfViewerModel
    {
        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        public string Url { get; set; }
    }
}
