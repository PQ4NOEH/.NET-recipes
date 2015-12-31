namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksIndexType
    {
        [JsonProperty(PropertyName = "exerciseType", Required = Required.Always)]
        public DesksIndexExerciseType Type { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }
    }
}
