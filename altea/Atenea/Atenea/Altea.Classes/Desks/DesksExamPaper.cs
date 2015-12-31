namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksExamPaper
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; } 

        [JsonProperty(PropertyName = "title", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "subtitle", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Subtitle { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "headerTitle", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string HeaderTitle { get; set; }

        [JsonProperty(PropertyName = "headerId", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? HeaderId { get; set; }

        [JsonProperty(PropertyName = "headerSubtitle", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string HeaderSubtitle { get; set; }

        [JsonProperty(PropertyName = "hasVocabulary", Required = Required.Always)]
        public bool HasVocabulary { get; set; }

        [JsonProperty(PropertyName = "forceVocabulary", Required = Required.Always)]
        public bool ForceVocabulary { get; set; }

        [JsonProperty(PropertyName = "examMode", Required = Required.Always)]
        public bool ExamMode { get; set; }

        [JsonProperty(PropertyName = "onlyExamMode", Required = Required.Always)]
        public bool OnlyExamMode { get; set; }

        [JsonProperty(PropertyName = "hasMixedExam", Required = Required.Always)]
        public bool HasMixedExam { get; set; }

        [JsonProperty(PropertyName = "info", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Info { get; set; }
    }
}