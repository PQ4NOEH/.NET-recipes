namespace Heracles.Models.WiseTank
{
    using System.Collections.Generic;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankTimelineBoxDataModel : WiseTankStreamBoxDataModel
    {
        [JsonProperty(PropertyName = "users", Required = Required.Always)]
        public IEnumerable<TankTimelineUser> Users { get; set; }
    }
}
