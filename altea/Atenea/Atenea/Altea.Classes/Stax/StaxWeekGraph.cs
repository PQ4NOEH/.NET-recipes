namespace Altea.Classes.Stax
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class StaxWeekGraph
    {
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "thisWeek", Required = Required.Always)]
        public IEnumerable<StackDayGraph> ThisWeek { get; set; }

        [JsonProperty(PropertyName = "lastWeek", Required = Required.Always)]
        public IEnumerable<StackDayGraph> LastWeek { get; set; }

        [JsonProperty(PropertyName = "averageWeek", Required = Required.Always)]
        public IEnumerable<StackDayGraph> AverageWeek { get; set; }

        public StaxWeekGraph()
        {
            this.ThisWeek = new List<StackDayGraph>(7);
            this.LastWeek = new List<StackDayGraph>(7);
            this.AverageWeek = new List<StackDayGraph>(7);
        }
    }
}
