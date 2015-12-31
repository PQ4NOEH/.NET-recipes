namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksBookType
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "subTypes", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksBookType> SubTypes { get; set; } 
    }
}
