namespace Altea.Classes.WiseTank
{
    using System;

    using Newtonsoft.Json;

    public class TankBox
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public TankBoxType Type { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "query", Required = Required.Always)]
        public string Query { get; set; }
    }
}
