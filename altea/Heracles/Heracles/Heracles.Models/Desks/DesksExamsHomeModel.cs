namespace Heracles.Models.Desks
{
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class DesksExamsHomeModel
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public Level Level { get; set; }
    }
}
