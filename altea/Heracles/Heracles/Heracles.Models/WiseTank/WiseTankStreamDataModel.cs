namespace Heracles.Models.WiseTank
{
    using System.Collections.Generic;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankStreamDataModel : WiseTankDataModel
    {
        [JsonProperty(PropertyName = "streams", Required = Required.Always)]
        public IEnumerable<TankStream> Streams { get; set; }

        [JsonProperty(PropertyName = "roles", Required = Required.Always)]
        public IEnumerable<TankRole> Roles { get; set; }
    }
}
