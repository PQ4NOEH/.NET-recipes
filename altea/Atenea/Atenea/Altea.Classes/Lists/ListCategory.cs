namespace Altea.Classes.Lists
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ListCategory
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isArea", Required = Required.Always)]
        public bool IsArea { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonIgnore]
        public ListCategory Parent { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<ListCategory> Children { get; set; }

        [JsonProperty(PropertyName = "listCount", Required = Required.Always)]
        public IDictionary<int, int> ListCount { get; set; } 
    }
}
