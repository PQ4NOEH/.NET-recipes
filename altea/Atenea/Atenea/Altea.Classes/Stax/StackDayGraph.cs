namespace Altea.Classes.Stax
{
    using Newtonsoft.Json;

    public class StackDayGraph
    {
        [JsonProperty(PropertyName = "weekDay", Required = Required.Always)]
        public int Weekday { get; set; }

        [JsonProperty(PropertyName = "count", Required = Required.Always)]
        public int Count { get; set; }
    }
}
