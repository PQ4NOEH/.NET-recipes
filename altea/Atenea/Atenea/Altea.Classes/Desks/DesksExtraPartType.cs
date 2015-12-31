namespace Altea.Classes.Desks
{
    using Altea.Extensions;

    using Newtonsoft.Json;

    public class DesksExtraPartType
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public int Type { get; set; }

        [JsonIgnore]
        public string MainData { get; set; }

        [JsonProperty(PropertyName = "mainData", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
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
        
    }
}
