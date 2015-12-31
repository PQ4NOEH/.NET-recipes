namespace Heracles.Models.Dean
{
    using System;

    using Altea.Classes.Dean;

    using Newtonsoft.Json;

    public class DeanIndexStickyModel
    {
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public Guid? Id { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DeanMemberType? Type { get; set; }

        [JsonProperty(PropertyName = "pro", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? Pro { get; set; }

        [JsonProperty(PropertyName = "level", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Level { get; set; }

        [JsonProperty(PropertyName = "subLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? SubLevel { get; set; }
    }
}
