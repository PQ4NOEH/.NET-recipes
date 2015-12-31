namespace Altea.Classes.WiseReader
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Folder
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonIgnore]
        public Guid? Parent { get; set; }

        [JsonIgnore]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<Folder> Children { get; set; } 
    }
}
