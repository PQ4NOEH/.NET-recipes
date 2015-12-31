namespace Altea.Classes.Desks
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    [DataContract]
    public class DesksFinishResult
    {
        [JsonProperty(PropertyName = "checkStatus", Required = Required.Always)]
        public DesksCheckStatus CheckStatus { get; set; }

        [JsonProperty(PropertyName = "hasAnalyse", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool HasAnalyse { get; set; }

        [JsonProperty(PropertyName = "isBlocked", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public bool IsBlocked { get; set; }
    }
}
