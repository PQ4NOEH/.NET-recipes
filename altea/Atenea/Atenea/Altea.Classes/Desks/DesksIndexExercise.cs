namespace Altea.Classes.Desks
{
    using Newtonsoft.Json;

    public class DesksIndexExercise
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "statement", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Statement { get; set; }

        [JsonProperty(PropertyName = "other", Required = Required.Always)]
        public dynamic Other { get; set; }
    }
}
