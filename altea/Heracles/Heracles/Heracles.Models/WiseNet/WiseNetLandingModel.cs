namespace Heracles.Models.WiseNet
{
    using System.Collections.Generic;

    using Altea.Classes.WiseNet;

    using Newtonsoft.Json;

    public class WiseNetLandingModel
    {
        [JsonProperty(PropertyName = "searchEngines", Required = Required.Always)]
        public IEnumerable<WiseNetSearchEngine> SearchEngines { get; set; }

        [JsonProperty(PropertyName = "magazines", Required = Required.Always)]
        public IEnumerable<WiseNetCarousel> Magazines { get; set; }
    }
}
