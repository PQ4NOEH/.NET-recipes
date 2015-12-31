using Newtonsoft.Json;
using System;

namespace AlteaLabs.WiseLab.Contracts
{
    public class WiseLabHuntData
    {
        [JsonIgnore]
        public WiseLabHuntType Type { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "sentence", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Sentence { get; set; }

        [JsonProperty(PropertyName = "searched", Required = Required.Always)]
        public bool Searched { get; set; }

        [JsonProperty(PropertyName = "huntDate", Required = Required.Always)]
        public DateTime? HuntDate { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public TranslatedWord Translations { get; set; }
    }
}
