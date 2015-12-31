namespace Heracles.Models.Stax
{
    using System.Collections.Generic;

    using Altea.Classes.Stax;

    using Newtonsoft.Json;

    public class StaxModel
    {
        [JsonProperty(PropertyName = "stax", Required = Required.Always)]
        public IEnumerable<Stack> Stax { get; set; }

        [JsonProperty(PropertyName = "finished", Required = Required.Always)]
        public int Finished { get; set; }

        [JsonProperty(PropertyName = "settings", Required = Required.Always)]
        public StackSettings Settings { get; set; }
    }
}
