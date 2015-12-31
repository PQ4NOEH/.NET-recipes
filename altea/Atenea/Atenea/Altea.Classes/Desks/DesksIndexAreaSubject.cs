namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexAreaSubject
    {
        [JsonProperty(PropertyName = "area", Required = Required.Always)]
        public int Area { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "theory", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Theory { get; set; }

        [JsonProperty(PropertyName = "boards", Required = Required.Always)]
        public IDictionary<DesksIndexExerciseType, int> Boards { get; set; } 
    }
}
