namespace Heracles.Models.WiseTank
{
    using Newtonsoft.Json;

    public class WiseTankRolePermissionTypeModel
    {
        [JsonProperty(PropertyName = "role", Required = Required.Always)]
        public int Role { get; set; }

        [JsonProperty(PropertyName = "permissionType", Required = Required.Always)]
        public int PermissionType { get; set; }
    }
}
