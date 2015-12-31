namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Altea.Extensions;

    using Newtonsoft.Json;

    public class DesksExtraPart
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "area", Required = Required.Always)]
        public int Area { get; set; }

        [JsonIgnore]
        public string MainData { get; set; }

        [JsonProperty(PropertyName = "mainData", Required = Required.Always)]
        public dynamic MainDataDynamic
        {
            get
            {
                return this.MainData == null ? null : this.MainData.FromJson<dynamic>();
            }
        }

        [JsonIgnore]
        public string ExtraData { get; set; }

        [JsonProperty(PropertyName = "extraData", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public dynamic ExtraDataDynamic
        {
            get
            {
                return this.ExtraData == null ? null : this.ExtraData.FromJson<dynamic>();
            }
        }

        [JsonProperty(PropertyName = "types", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksExtraPartType> Types { get; set; }
    }
}
