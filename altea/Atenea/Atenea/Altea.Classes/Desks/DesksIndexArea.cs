namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexArea
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "subjects", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<int> Subjects { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "column", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Column { get; set; }

        [JsonProperty(PropertyName = "row", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Row { get; set; }

        [JsonProperty(PropertyName = "rowSize", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? RowSize { get; set; }
    }
}
