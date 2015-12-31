namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamTestAssignment : IDesksExamAssignment
    {
        [JsonProperty(PropertyName = "paper", Required = Required.Always)]
        public int Paper { get; set; }

        [JsonProperty(PropertyName = "group", Required = Required.Always)]
        public int Group { get; set; }

        [JsonProperty(PropertyName = "test", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Test { get; set; }

        [JsonProperty(PropertyName = "round", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Round { get; set; }

        [JsonProperty(PropertyName = "parts", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<int> Parts { get; set; }

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
