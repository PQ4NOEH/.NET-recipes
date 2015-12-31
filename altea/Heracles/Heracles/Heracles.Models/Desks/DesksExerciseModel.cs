namespace Heracles.Models.Desks
{
    using Newtonsoft.Json;

    public class DesksExerciseModel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "round", Required = Required.Always)]
        public int Round { get; set; }

        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "subtitle", Required = Required.Always)]
        public string Subtitle { get; set; }

        [JsonIgnore]
        public string ViewName { get; set; }
    }
}
