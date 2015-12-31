namespace Heracles.Models.WiseLab
{
    using Altea.Classes.WiseLab;

    using Newtonsoft.Json;

    public class WiseLabArticleDataRequestModel
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public WiseLabError Error { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public TranslatedWord Translations { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public WiseLabStatus? Status { get; set; }
    }
}
