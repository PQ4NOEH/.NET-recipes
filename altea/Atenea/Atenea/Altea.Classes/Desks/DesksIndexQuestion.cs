namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexQuestion
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "exercise", Required = Required.Always)]
        public int Exercise { get; set; }

        [JsonProperty(PropertyName = "question", Required = Required.Always)]
        public string Question { get; set; }

        [JsonProperty(PropertyName = "extraData", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string ExtraData { get; set; }

        [JsonProperty(PropertyName = "numGaps", Required = Required.Always)]
        public int NumGaps { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "auxiliarWords", Required = Required.Always)]
        public IEnumerable<string> AuxiliarWords { get; set; }

        [JsonProperty(PropertyName = "answers", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<DesksIndexAnswer> Answers { get; set; }

        [JsonProperty(PropertyName = "length", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int Length { get; set; } 

        [JsonProperty(PropertyName = "reported", Required = Required.Always)]
        public DesksReportStatus Reported { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.Always)]
        public IEnumerable<DesksIndexQuestion> Children { get; set; } 
    }
}
