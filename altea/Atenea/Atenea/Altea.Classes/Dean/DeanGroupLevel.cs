namespace Altea.Classes.Dean
{
    using System;

    using Newtonsoft.Json;

    public class DeanGroupLevel
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "active", Required = Required.Always)]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "startDate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DateTime? EndDate { get; set; }
    }
}
