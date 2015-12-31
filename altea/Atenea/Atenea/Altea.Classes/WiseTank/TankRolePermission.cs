namespace Altea.Classes.WiseTank
{
    using Newtonsoft.Json;

    public class TankRolePermission
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "permissions", Required = Required.Always)]
        public TankPermissions Permissions { get; set; }
    }
}
