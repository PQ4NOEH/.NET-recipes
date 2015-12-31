namespace Altea.Classes.Stax.WordStax
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.WiseLab;

    using Newtonsoft.Json;

    public class WordStaxInboxData : IStackInboxData
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "origin", Required = Required.Always)]
        public int Origin { get; set; }

        [JsonProperty(PropertyName = "insertDate", Required = Required.Always)]
        public DateTime InsertDate { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "numErrors", Required = Required.Always)]
        public int NumErrors { get; set; }

        [JsonProperty(PropertyName = "reinserted", Required = Required.Always)]
        public bool Reinserted { get; set; }

        [JsonProperty(PropertyName = "dataInStax", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<string> DataInStax { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public TranslatedWord Translations { get; set; }
    }
}
