namespace Altea.Classes.Stats
{
    using Newtonsoft.Json;

    public class ModuleStatsData
    {
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value", Required = Required.Always)]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public ModuleStatsStatus Status { get; set; }
    }
}
