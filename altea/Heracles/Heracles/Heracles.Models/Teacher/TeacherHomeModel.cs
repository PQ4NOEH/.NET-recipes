namespace Heracles.Models.Teacher
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.Desks;

    using Newtonsoft.Json;

    public class TeacherHomeModel
    {
        [JsonProperty(PropertyName = "areas", Required = Required.Always)]
        public IEnumerable<DesksIndexArea> Areas { get; set; }

        [JsonProperty(PropertyName = "extra", Required = Required.Always)]
        public dynamic Extra { get; set; }
        
        [JsonProperty(PropertyName = "level", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Level { get; set; }

        [JsonProperty(PropertyName = "group", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public Guid? Group { get; set; }

    }
}