namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksBookCollection : IDesksBookData
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "exerciseTypes", Required = Required.Always)]
        public IEnumerable<int> ExerciseTypes { get; set; }

        [JsonProperty(PropertyName = "categories", Required = Required.Always)]
        public IEnumerable<int> Categories { get; set; }

        [JsonProperty(PropertyName = "tags", Required = Required.Always)]
        public IEnumerable<int> Tags { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "publications", Required = Required.Always)]
        public IDictionary<int, DesksBookPublication> Publications { get; set; }
    }
}
