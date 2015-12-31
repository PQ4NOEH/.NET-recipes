namespace Heracles.Models.Stax
{
    using Newtonsoft.Json;

    public class StaxStackModel
    {
        [JsonProperty(PropertyName = "stackNum", Required = Required.Always)]
        public int StackNum { get; set; }

        [JsonProperty(PropertyName = "maxStack", Required = Required.Always)]
        public int MaxStack { get; set; }

        [JsonProperty(PropertyName = "timeLimit", Required = Required.Always)]
        public int TimeLimit { get; set; }
    }
}
