namespace Heracles.Models.Stax
{
    using Altea.Classes.WiseLab;

    using Newtonsoft.Json;

    public class WordStaxSearchModel
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public WiseLabError Error { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public TranslatedWord Translations { get; set; }

    }
}
