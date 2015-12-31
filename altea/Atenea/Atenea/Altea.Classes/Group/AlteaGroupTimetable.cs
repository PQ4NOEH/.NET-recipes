namespace Altea.Classes.Group
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AlteaGroupTimetable
    {
        [JsonProperty(PropertyName = "weekday", Required = Required.Always)]
        public int Weekday { get; set; }

        [JsonProperty(PropertyName = "hour", Required = Required.Always)]
        public int Hour { get; set; }

        [JsonProperty(PropertyName = "minute", Required =Required.Always)]
        public int Minute { get; set; }

        [JsonProperty(PropertyName = "duration", Required = Required.Always)]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "classroom", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Classroom { get; set; }

        [JsonProperty(PropertyName = "teachers", Required = Required.Always)]
        public IEnumerable<Guid> Teachers { get; set; } 
    }
}
