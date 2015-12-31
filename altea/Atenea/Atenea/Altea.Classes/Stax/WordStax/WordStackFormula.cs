namespace Altea.Classes.Stax.WordStax
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class WordStackFormula : IStackFormula
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "answer", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Answer { get; set; }

        [JsonProperty(PropertyName = "otherData", Required = Required.Always)]
        public IEnumerable<string> OtherData { get; set; }
    }
}
