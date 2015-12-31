namespace Altea.Classes.Members
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.Group;

    using Newtonsoft.Json;

    public class AlteaGroup
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<AlteaGroupLevel> Levels { get; set; }
    }
}
