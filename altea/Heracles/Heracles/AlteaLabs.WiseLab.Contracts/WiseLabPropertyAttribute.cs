using Newtonsoft.Json;
using System;

namespace AlteaLabs.WiseLab.Contracts
{
    public sealed class WiseLabPropertyAttribute : Attribute
    {
        [JsonIgnore]
        public override object TypeId
        {
            get
            {
                return base.TypeId;
            }
        }

        [JsonProperty(PropertyName = "maxStatus", Required = Required.Always)]
        public WiseLabStatus MaxStatus { get; set; }

        [JsonProperty(PropertyName = "hasPod", Required = Required.Always)]
        public bool HasPod { get; set; }

        [JsonProperty(PropertyName = "canForcePod", Required = Required.Always)]
        public bool CanForcePod { get; set; }

        [JsonProperty(PropertyName = "hasPlanner", Required = Required.Always)]
        public bool HasPlanner { get; set; }

        [JsonProperty(PropertyName = "minForcedScoutedWords", Required = Required.Always)]
        public int MinForcedScoutedWords { get; set; }

        [JsonProperty(PropertyName = "minForcedKeywords", Required = Required.Always)]
        public int MinForcedKeywords { get; set; }

        [JsonProperty(PropertyName = "minKeywords", Required = Required.Always)]
        public int MinKeywords { get; set; }

        [JsonProperty(PropertyName = "maxKeywords", Required = Required.Always)]
        public int MaxKeywords { get; set; }

        [JsonProperty(PropertyName = "minForcedExpressions", Required = Required.Always)]
        public int MinForcedExpressions { get; set; }

        [JsonProperty(PropertyName = "minExpressions", Required = Required.Always)]
        public int MinExpressions { get; set; }

        [JsonProperty(PropertyName = "maxExpressions", Required = Required.Always)]
        public int MaxExpressions { get; set; }

        [JsonProperty(PropertyName = "minLead", Required = Required.Always)]
        public int MinLead { get; set; }

        [JsonProperty(PropertyName = "maxLead", Required = Required.Always)]
        public int MaxLead { get; set; }
    }
}
