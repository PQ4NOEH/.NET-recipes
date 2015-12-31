namespace Heracles.Models.WiseTank
{
    using System;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankCreateStatusModel
    {
        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public WiseTankError Status { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public Guid? Id { get; set; }
    }
}
