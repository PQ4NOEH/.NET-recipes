namespace Altea.Classes.Lists
{
    using Newtonsoft.Json;

    public class AssignedListStatus
    {
        [JsonIgnore]
        public int SectionId { get; set; }

        [JsonProperty(PropertyName = "section", Required = Required.Always)]
        public string Section { get; set; }

        [JsonIgnore]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = "category", Required = Required.Always)]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "assigned", Required = Required.Always)]
        public int Assigned { get; set; }

        [JsonProperty(PropertyName = "inboxed", Required = Required.Always)]
        public int Inboxed { get; set; }

        [JsonProperty(PropertyName = "accepted", Required = Required.Always)]
        public int Accepted { get; set; }

        [JsonProperty(PropertyName = "rejected", Required = Required.Always)]
        public int Rejected { get; set; }

        [JsonProperty(PropertyName = "workedAndRejected", Required = Required.Always)]
        public int WorkedAndRejected { get; set; }
        
        [JsonProperty(PropertyName = "finished", Required = Required.Always)]
        public int Finished { get; set; }

        [JsonProperty(PropertyName = "recognized", Required = Required.Always)]
        public int Recognized { get; set; }
    }
}
