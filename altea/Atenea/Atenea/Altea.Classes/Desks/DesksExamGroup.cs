namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamGroup
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "subtitle", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Subtitle { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "papers", Required = Required.Always)]
        public IEnumerable<DesksExamPaper> Papers { get; set; } 
    }
}
