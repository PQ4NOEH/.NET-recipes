namespace Altea.Classes.WiseTank
{
    using System;

    using Newtonsoft.Json;

    public class TankTimelineUserData
    {
        [JsonProperty(PropertyName = "timelineId", Required = Required.Always)]
        public Guid TimelineId { get; set; }

        [JsonProperty(PropertyName = "permissionType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? PermissionType { get; set; }

        [JsonProperty(PropertyName = "thinktankLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? ThinktankLevel { get; set; }

        [JsonProperty(PropertyName = "role", Required = Required.Always)]
        public int Role { get; set; }

        [JsonProperty(PropertyName = "permissions", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public TankPermissions Permissions { get; set; }
    }
}
