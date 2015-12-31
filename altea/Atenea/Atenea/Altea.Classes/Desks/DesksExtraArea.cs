namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Altea.Extensions;

    using Newtonsoft.Json;

    public class DesksExtraArea
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonIgnore]
        public string ExtraData { get; set; }

        [JsonProperty(PropertyName = "extraData", Required = Required.AllowNull,
            NullValueHandling = NullValueHandling.Include)]
        public dynamic ExtraDataDynamic
        {
            get
            {
                return this.ExtraData == null ? null : this.ExtraData.FromJson<dynamic>();
            }
        }

        [JsonProperty(PropertyName = "subAreas", Required = Required.Always)]
        public IEnumerable<DesksExtraArea> SubAreas { get; set; } 

        [JsonProperty(PropertyName = "types", Required = Required.Always)]
        public IEnumerable<DesksExtraType> Types { get; set; } 
    }
}
