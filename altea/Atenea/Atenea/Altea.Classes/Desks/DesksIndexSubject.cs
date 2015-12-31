namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexSubject
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "areas", Required = Required.Always)]
        public IList<DesksIndexAreaSubject> Areas { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public IList<DesksIndexSubject> Children { get; set; }
    }
}
