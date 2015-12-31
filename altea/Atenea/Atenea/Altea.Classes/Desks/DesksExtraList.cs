namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExtraList
    {
        [JsonProperty(PropertyName = "areas", Required = Required.Always)]
        public IEnumerable<DesksExtraArea> Areas { get; set; }

        [JsonProperty(PropertyName = "parts", Required = Required.Always)]
        public IEnumerable<DesksExtraPart> Parts { get; set; }
    }
}
