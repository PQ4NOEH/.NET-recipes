namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamData
    {
        [JsonProperty(PropertyName = "paper", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Paper { get; set; }

        [JsonProperty(PropertyName = "paperType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string PaperType { get; set; }

        [JsonProperty(PropertyName = "statement", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Statement { get; set; }

        [JsonProperty(PropertyName = "content", Required = Required.Always)]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "extraData", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public dynamic ExtraData { get; set; }

        [JsonProperty(PropertyName = "possibleAnswers", Required = Required.Always)]
        public IEnumerable<string> PossibleAnswers { get; set; }
        
        [JsonProperty(PropertyName = "questions", Required = Required.Always)]
        public IEnumerable<DesksExamQuestion> Questions { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.Always)]
        public IEnumerable<DesksExamData> Children { get; set; }
    }
}