namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksAssignmentRoundStats
    {
        [JsonProperty(PropertyName = "round", Required = Required.Always)]
        public int Round { get; set; }

        [JsonProperty(PropertyName = "validAnswers", Required = Required.Always)]
        public int ValidAnswers { get; set; }

        [JsonProperty(PropertyName = "invalidAnswers", Required = Required.Always)]
        public int InvalidAnswers { get; set; }

        [JsonProperty(PropertyName = "time", Required = Required.Always)]
        public int Time { get; set; }
    }
}
