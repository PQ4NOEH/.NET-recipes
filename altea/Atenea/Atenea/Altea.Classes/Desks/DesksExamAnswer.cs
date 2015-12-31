namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksExamAnswer
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "answer", Required = Required.Always)]
        public string Answer { get; set; }

        [JsonProperty(PropertyName = "gap", Required = Required.Always)]
        public int Gap { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "valid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Valid { get; set; }
    }
}
