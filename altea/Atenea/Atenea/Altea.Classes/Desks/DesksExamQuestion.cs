namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamQuestion
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "statement", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Statement { get; set; }

        [JsonProperty(PropertyName = "question", Required = Required.Always)]
        public string Question { get; set; }

        [JsonProperty(PropertyName = "numGaps", Required = Required.Always)]
        public int NumGaps { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "extraData", Required = Required.Always)]
        public dynamic ExtraData { get; set; }

        [JsonProperty(PropertyName = "auxiliarWords", Required = Required.Always)]
        public IEnumerable<string> AuxiliarWords { get; set; } 

        [JsonProperty(PropertyName = "answers", Required = Required.Always)]
        public IEnumerable<DesksExamAnswer> Answers { get; set; } 

        [JsonProperty(PropertyName = "reported", Required = Required.Always)]
        public DesksReportStatus Reported { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.Always)]
        public IEnumerable<DesksExamQuestion> Children { get; set; } 
    }
}
