namespace Altea.Classes.WiseTank
{
    using System;
    using System.Collections.Generic;

    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class TankTimeline
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "area", Required = Required.Always)]
        public TankArea Area { get; set; }

        [JsonProperty(PropertyName = "category", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Category { get; set; }

        [JsonProperty(PropertyName = "isAppCategory", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? IsAppCategory { get; set; }

        [JsonProperty(PropertyName = "language", Required = Required.Always)]
        public Language Language { get; set; }

        [JsonProperty(PropertyName = "accessType", Required = Required.Always)]
        public TankAccessType AccessType { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<TankTimeline> Children { get; set; }

        [JsonIgnore]
        public Guid? CreatedById { get; set; }

        [JsonProperty(PropertyName = "createdBy", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "createDate", Required = Required.Always)]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "userEditable", Required = Required.Always)]
        public bool UserEditable { get; set; }

        [JsonProperty(PropertyName = "areaDefault", Required = Required.Always)]
        public bool AreaDefault { get; set; }

        [JsonProperty(PropertyName = "moderatedArticles", Required = Required.Always)]
        public bool ModeratedArticles { get; set; }

        [JsonProperty(PropertyName = "moderatedComments", Required = Required.Always)]
        public bool ModeratedComments { get; set; }

        [JsonProperty(PropertyName = "writeOwnArticles", Required = Required.Always)]
        public bool WriteOwnArticles { get; set; }

        [JsonProperty(PropertyName = "role", Required = Required.Always)]
        public int Role { get; set; }

        [JsonProperty(PropertyName = "permissionType", Required = Required.Always)]
        public int PermissionType { get; set; }

        [JsonProperty(PropertyName = "customPermission", Required = Required.Always)]
        public bool CustomPermission { get; set; }

        [JsonProperty(PropertyName = "defaultPermission", Required = Required.Always)]
        public bool DefaultPermission { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "refreshRate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? RefreshRate { get; set; }

        [JsonProperty(PropertyName = "boxesWidth", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? BoxesWidth { get; set; }

        [JsonProperty(PropertyName = "userThinktank", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? UserThinktank { get; set; }

        [JsonProperty(PropertyName = "thinktankLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? ThinktankLevel { get; set; }

        [JsonProperty(PropertyName = "permissions", Required = Required.Always)]
        public TankPermissions Permissions { get; set; }

        [JsonProperty(PropertyName = "permissionTypes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<int, int> PermissionTypes { get; set; }
    }
}
