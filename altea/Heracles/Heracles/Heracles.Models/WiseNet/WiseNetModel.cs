namespace Heracles.Models.WiseNet
{
    using Newtonsoft.Json;

    public class WiseNetModel
    {
        [JsonProperty(PropertyName = "defaultSearchUrl", Required = Required.Always)]
        public string DefaultSearchUrl { get; set; }
    }
}
