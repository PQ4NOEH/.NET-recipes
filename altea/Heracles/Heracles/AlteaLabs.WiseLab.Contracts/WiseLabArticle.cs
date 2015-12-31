using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AlteaLabs.WiseLab.Contracts
{
    public class WiseLabArticle
    {
        [JsonProperty(PropertyName = "origin", Required = Required.Always)]
        public WiseLabOrigin Origin { get; set; }

        [JsonProperty(PropertyName = "reference", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Reference { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public WiseLabStatus Status { get; set; }

        [JsonProperty(PropertyName = "lead", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Lead { get; set; }

        [JsonProperty(PropertyName = "forcePod", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ForcePod { get; set; }



        private ILookup<WiseLabHuntType, WiseLabHuntData> huntData;

        [JsonIgnore]
        public IEnumerable<WiseLabHuntData> HuntData
        {
            set
            {
                this.huntData = value.ToLookup(x => x.Type);
            }
        }

        [JsonProperty(PropertyName = "scouted", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<WiseLabHuntData> Scouted
        {
            get
            {
                return this.huntData == null ? null : this.huntData[WiseLabHuntType.Scout];
            }
        }

        [JsonProperty(PropertyName = "keywords", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<WiseLabHuntData> Keywords
        {
            get
            {
                return this.huntData == null ? null : this.huntData[WiseLabHuntType.Keyword];
            }
        }

        [JsonProperty(PropertyName = "expressions", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<WiseLabHuntData> Expressions
        {
            get
            {
                return this.huntData == null ? null : this.huntData[WiseLabHuntType.Expression];
            }
        }
    }
}