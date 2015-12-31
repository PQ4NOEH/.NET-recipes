namespace Altea.Classes.Dean
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DeanUser
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "userName", Required = Required.Always)]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<int> Levels { get; set; }

        [JsonProperty(PropertyName = "proLevels", Required = Required.Always)]
        public IEnumerable<Tuple<int, int>>  ProLevels { get; set; }

        [JsonProperty(PropertyName = "primaryLevel", Required = Required.Always)]
        public int PrimaryLevel { get; set; }

        [JsonProperty(PropertyName = "primaryProLevel", Required = Required.Always)]
        public Tuple<int, int> PrimaryProLevel { get; set; }
    }
}
