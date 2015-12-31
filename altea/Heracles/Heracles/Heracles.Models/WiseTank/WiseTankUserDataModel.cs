namespace Heracles.Models.WiseTank
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankUserDataModel
    {
        [JsonProperty(PropertyName = "userData", Required = Required.Always)]
        public IDictionary<Guid, IEnumerable<TankTimelineUser>> UserData { get; set; }

        [JsonProperty(PropertyName = "hasPermissions", Required = Required.Always)]
        public IDictionary<Guid, bool> HasPermissions { get; set; }
        
        [JsonProperty(PropertyName = "permissions", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<int, int> Permissions { get; set; }
    }
}
