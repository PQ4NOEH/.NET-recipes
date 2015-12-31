namespace Altea.Classes.Stax.WordStax
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ExtendedWordStackFormula : IStackFormula
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public IEnumerable<WordStackFormula> Data { get; set; }
        
        [JsonProperty(PropertyName = "otherData", Required = Required.Always)]
        public IEnumerable<string> OtherData { get; set; }
    }
}
