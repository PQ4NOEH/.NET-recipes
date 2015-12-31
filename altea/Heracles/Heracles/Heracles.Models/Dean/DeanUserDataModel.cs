namespace Heracles.Models.Dean
{
    using System.Collections.Generic;

    using Altea.Classes.Desks;
    using Altea.Classes.Lists;
    using Altea.Classes.ProDesks;
    using Altea.Classes.WiseNet;

    using Newtonsoft.Json;

    public class DeanUserDataModel
    {
        [JsonProperty(PropertyName = "desksIndexAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksIndexAssignment> DesksIndexAssignments { get; set; }

        [JsonProperty(PropertyName = "desksExamsAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<IDesksExamAssignment> DesksExamsAssignments { get; set; }

        [JsonProperty(PropertyName = "desksBooksAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksBookAssignment> DesksBooksAssignments { get; set; }

        [JsonProperty(PropertyName = "proDesksAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<IProDesksAssignment> ProDesksAssignments { get; set; }

        [JsonProperty(PropertyName = "searchEngines", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<WiseNetSearchEngine> SearchEngines { get; set; }

        [JsonProperty(PropertyName = "magazines", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<WiseNetCarousel> Magazines { get; set; }

        [JsonProperty(PropertyName = "lists", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<AssignedList> Lists { get; set; }
    }
}
