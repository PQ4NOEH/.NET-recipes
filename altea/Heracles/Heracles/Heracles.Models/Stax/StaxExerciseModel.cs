namespace Heracles.Models.Stax
{
    using Altea.Classes.Stax;

    using Newtonsoft.Json;

    public class StaxExerciseModel
    {
        [JsonProperty(PropertyName = "stackStatus", Required = Required.Always)]
        public StackStatus Status { get; set; }

        [JsonProperty(PropertyName = "formula", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IStackFormula Formula { get; set; }
    }
}
