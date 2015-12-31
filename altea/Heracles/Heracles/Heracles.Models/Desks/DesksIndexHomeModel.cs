namespace Heracles.Models.Desks
{
    using System.Collections.Generic;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class DesksIndexHomeModel
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public Level Level { get; set; }

        [JsonProperty(PropertyName = "areas", Required = Required.Always)]
        public IEnumerable<DesksIndexArea> Areas { get; set; }
    }
}
