namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksExamPartAssignment : IDesksExamAssignment
    {
        [JsonProperty(PropertyName = "part", Required = Required.Always)]
        public int Part { get; set; }

        [JsonProperty(PropertyName = "vocabulary", Required = Required.Always)]
        public bool Vocabulary { get; set; }

        [JsonProperty(PropertyName = "assigned", Required = Required.Always)]
        public bool Assigned { get; set; }

        [JsonProperty(PropertyName = "remoteAssignment", Required = Required.Always)]
        public bool RemoteAssignment { get; set; }

        [JsonProperty(PropertyName = "blocked", Required = Required.Always)]
        public bool Blocked { get; set; }

        [JsonProperty(PropertyName = "finished", Required = Required.Always)]
        public bool Finished { get; set; }

        [JsonProperty(PropertyName = "certified", Required = Required.Always)]
        public bool Certified { get; set; }
    }
}
