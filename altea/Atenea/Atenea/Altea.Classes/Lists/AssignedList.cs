namespace Altea.Classes.Lists
{
    using System;

    using Newtonsoft.Json;

    public class AssignedList
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "inboxFactor", Required = Required.Always)]
        public int InboxFactor { get; set; }

        [JsonProperty(PropertyName = "dataCount", Required = Required.Always)]
        public int DataCount { get; set; }

        [JsonProperty(PropertyName = "groupType", Required = Required.Always)]
        public int GroupType { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public AssignedListStatus Status { get; set; }
    }
}
