namespace Altea.Classes.WiseNet
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class WiseNetCarousel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "visible", Required = Required.Always)]
        public bool Visible { get; set; }

        [JsonProperty(PropertyName = "magazines", Required = Required.Always)]
        public ICollection<WiseNetMagazine> Magazines { get; set; } 
    }
}
