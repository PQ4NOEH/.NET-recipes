namespace Heracles.Models.ProDesks
{
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class ProDesksHomeModel
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public ProLevel Level { get; set; }

        [JsonProperty(PropertyName = "columns", Required = Required.Always)]
        public dynamic Columns { get; set; }
    }
}
