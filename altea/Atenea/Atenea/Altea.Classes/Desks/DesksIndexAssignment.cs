namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksIndexAssignment
    {
        [JsonProperty(PropertyName = "area", Required = Required.Always)]
        public int Area { get; set; }

        [JsonProperty(PropertyName = "subject", Required = Required.Always)]
        public int Subject { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public DesksIndexExerciseType Type { get; set; }

        [JsonProperty(PropertyName = "assigned", Required = Required.Always)]
        public int Assigned { get; set; }

        [JsonProperty(PropertyName = "remoteAssignment", Required = Required.Always)]
        public int RemoteAssignment { get; set; }

        [JsonProperty(PropertyName = "blocked", Required = Required.Always)]
        public int Blocked { get; set; }

        [JsonProperty(PropertyName = "finished", Required = Required.Always)]
        public int Finished { get; set; }

        [JsonProperty(PropertyName = "certified", Required = Required.Always)]
        public int Certified { get; set; }
    }
}
