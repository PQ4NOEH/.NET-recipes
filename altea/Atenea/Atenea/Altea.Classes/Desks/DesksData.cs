namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public abstract class DesksData
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "view", Required = Required.Always)]
        public string View { get; set; }

        [JsonProperty(PropertyName = "round", Required = Required.Always)]
        public int Round { get; set; }

        [JsonProperty(PropertyName = "maxRound", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int MaxRound { get; set; }

        [JsonProperty(PropertyName = "maxAnalyseRound", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int MaxAnalyseRound { get; set; }

        [JsonProperty(PropertyName = "time", Required = Required.Always)]
        public int Time { get; set; }
    }
}
