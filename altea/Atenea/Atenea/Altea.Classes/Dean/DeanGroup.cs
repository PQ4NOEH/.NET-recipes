namespace Altea.Classes.Dean
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DeanGroup
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "groupName", Required = Required.Always)]
        public string GroupName { get; set; }

        [JsonProperty(PropertyName = "active", Required = Required.Always)]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<DeanGroupLevel> Levels { get; set; }

        [JsonProperty(PropertyName = "primaryLevel", Required = Required.Always)]
        public int PrimaryLevel { get; set; }
    }
}
