namespace Altea.Classes.WiseTank
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TankRole
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; } 

        [JsonProperty(PropertyName = "selectable", Required = Required.Always)]
        public bool Selectable { get; set; } 

        [JsonProperty(PropertyName = "permissions", Required = Required.Always)]
        public IEnumerable<TankRolePermission> Permissions { get; set; }
    }
}