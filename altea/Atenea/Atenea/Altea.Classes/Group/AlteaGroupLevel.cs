namespace Altea.Classes.Group
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AlteaGroupLevel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "primary", Required = Required.Always)]
        public bool Primary { get; set; }

        [JsonProperty(PropertyName = "active", Required = Required.Always)]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "startDate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DateTime? EndDate { get; set; }

        [JsonProperty(PropertyName = "notes", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Notes { get; set; }

        [JsonProperty(PropertyName = "examCall", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? ExamCall { get; set; }

        [JsonProperty(PropertyName = "planningFinished", Required = Required.Always)]
        public bool PlanningFinished { get; set; }

        [JsonProperty(PropertyName = "timetable", Required = Required.Always)]
        public IEnumerable<AlteaGroupTimetable> Timetable { get; set; } 
    }
}
