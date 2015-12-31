namespace Altea.Classes.Stax
{
    using Newtonsoft.Json;

    public class StackSettings
    {
        [JsonProperty(PropertyName = "underflow", Required = Required.Always)]
        public int Underflow { get; set; }

        [JsonProperty(PropertyName = "overflow", Required = Required.Always)]
        public int Overflow { get; set; }

        [JsonProperty(PropertyName = "numStax", Required = Required.Always)]
        public int NumStax { get; set; }

        [JsonProperty(PropertyName = "steps", Required = Required.Always)]
        public int Steps { get; set; }
    }
}
