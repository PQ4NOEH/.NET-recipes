using Newtonsoft.Json;
using System.Collections.Generic;

namespace AlteaLabs.WiseLab.Contracts
{
    public class Translation
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.Always)]
        public IEnumerable<string> Translations { get; set; }
    }
}