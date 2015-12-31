namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksExamTest
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "groupId", Required = Required.Always)]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "parts", Required = Required.Always)]
        public IEnumerable<DesksExamTestPart> Parts { get; set; }
    }
}