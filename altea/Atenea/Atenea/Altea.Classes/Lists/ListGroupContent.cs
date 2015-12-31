namespace Altea.Classes.Lists
{
    using Newtonsoft.Json;

    public class ListGroupContent
    {
        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "dataSplit", Required = Required.Always)]
        public int DataSplit { get; set; }

        [JsonProperty(PropertyName = "percentageSplit", Required = Required.Always)]
        public bool PercentageSplit { get; set; }

        [JsonProperty(PropertyName = "splitCount", Required = Required.Always)]
        public int SplitCount { get; set; }
    }
}
