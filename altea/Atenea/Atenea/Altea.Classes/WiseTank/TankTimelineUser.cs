namespace Altea.Classes.WiseTank
{
    using System;

    using Newtonsoft.Json;

    public class TankTimelineUser
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "userName", Required = Required.Always)]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "fullName", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "role", Required = Required.Always)]
        public int Role { get; set; }

        [JsonProperty(PropertyName = "permissionType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? PermissionType { get; set; }

        [JsonProperty(PropertyName = "customPermission", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? CustomPermission { get; set; }

        [JsonProperty(PropertyName = "defaultPermission", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? DefaultPermission { get; set; }

        [JsonProperty(PropertyName = "thinktankLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? ThinktankLevel { get; set; }

        [JsonProperty(PropertyName = "permissions", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public TankPermissions Permissions { get; set; }
    }
}
