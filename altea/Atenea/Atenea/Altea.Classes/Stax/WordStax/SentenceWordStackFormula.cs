namespace Altea.Classes.Stax.WordStax
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class SentenceWordStackFormula : IStackFormula
    {
        [JsonProperty(PropertyName = "ids", Required = Required.Always)]
        public IEnumerable<int> Ids { get; set; }

        [JsonProperty(PropertyName = "datas", Required = Required.Always)]
        public IEnumerable<string> Datas { get; set; }

        [JsonProperty(PropertyName = "sentences", Required = Required.Always)]
        public IEnumerable<string> Sentences { get; set; } 
    }
}
