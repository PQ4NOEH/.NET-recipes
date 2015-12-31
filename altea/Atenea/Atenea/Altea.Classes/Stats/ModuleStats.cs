namespace Altea.Classes.Stats
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ModuleStats
    {
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "stats", Required = Required.Always)]
        public IEnumerable<ModuleStatsData> Stats { get; set; }
    }
}
