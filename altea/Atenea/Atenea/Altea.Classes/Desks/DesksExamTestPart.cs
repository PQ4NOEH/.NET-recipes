namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksExamTestPart
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "paper", Required = Required.Always)]
        public int Paper { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "hasVocabulary", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? HasVocabulary { get; set; }

        [JsonProperty(PropertyName = "forceVocabulary", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ForceVocabulary { get; set; }

        [JsonProperty(PropertyName = "allowInExamMode", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? AllowInExamMode { get; set; }

        [JsonProperty(PropertyName = "allowInMixedMode", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? AllowInMixedExam { get; set; }
    }
}