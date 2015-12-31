namespace Heracles.Models.WiseTank
{
    using System.Collections.Generic;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankDataModel
    {
        [JsonProperty(PropertyName = "areas", Required = Required.Always)]
        public IDictionary<string, int> Areas { get; set; }

        [JsonProperty(PropertyName = "timelines", Required = Required.Always)]
        public IEnumerable<TankTimeline> Timelines { get; set; } 
    }
}
