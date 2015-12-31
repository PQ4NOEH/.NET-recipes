namespace Altea.Classes.Lists
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AlteaList
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public ListType Type { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "tags", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty(PropertyName = "weight", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public double? Weight { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "count", Required = Required.Always)]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "categories", Required = Required.Always)]
        public IEnumerable<int> Categories { get; set; }
    }
}
