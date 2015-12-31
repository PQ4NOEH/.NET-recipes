namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamList
    {
        [JsonProperty(PropertyName = "groups", Required = Required.Always)]
        public IEnumerable<DesksExamGroup> Groups { get; set; }
        
        [JsonProperty(PropertyName = "tests", Required = Required.Always)]
        public IEnumerable<DesksExamTest> Tests { get; set; } 
    }
}
