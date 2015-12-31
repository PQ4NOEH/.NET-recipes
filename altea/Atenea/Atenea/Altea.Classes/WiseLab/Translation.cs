namespace Altea.Classes.WiseLab
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Translation
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "translations", Required = Required.Always)]
        public IEnumerable<string> Translations { get; set; } 
    }
}
