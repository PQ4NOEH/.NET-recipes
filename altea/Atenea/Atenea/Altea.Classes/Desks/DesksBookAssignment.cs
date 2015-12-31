namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksBookAssignment
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public DesksBookAssignmentType Type { get; set; }

        [JsonProperty(PropertyName = "publication", Required = Required.Always)]
        public int Publication { get; set; }

        [JsonProperty(PropertyName = "exerciseType", Required = Required.Always)]
        public int ExerciseType { get; set; }

        [JsonProperty(PropertyName = "exerciseSubType", Required = Required.Always)]
        public int ExerciseSubType { get; set; }

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
