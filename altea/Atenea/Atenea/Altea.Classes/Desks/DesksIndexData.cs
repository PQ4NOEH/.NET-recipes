namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexData
    {
        [JsonIgnore]
        public DesksIndexExerciseType ExerciseType { get; set; }

        [JsonIgnore]
        public int Round { get; set; }

        [JsonProperty(PropertyName = "time", Required = Required.Always)]
        public int Time { get; set; }

        [JsonProperty(PropertyName = "exercises", Required = Required.Always)]
        public IEnumerable<DesksIndexExercise> Exercises { get; set; }
        
        [JsonProperty(PropertyName = "questions", Required = Required.Always)]
        public IEnumerable<DesksIndexQuestion> Questions { get; set; } 
    }
}
