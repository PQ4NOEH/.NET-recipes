namespace Heracles.Models.Dean
{
    using System.Collections.Generic;

    using Altea.Classes.Desks;
    using Altea.Classes.Group;

    using Newtonsoft.Json;

    public class DeanGroupDataModel
    {
        [JsonProperty(PropertyName = "planning", Required = Required.Always)]
        public AlteaGroupPlanning Planning { get; set; }

        [JsonProperty(PropertyName = "desksIndexAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksIndexAssignment> DesksIndexAssignments { get; set; }

        [JsonProperty(PropertyName = "desksExamsAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<IDesksExamAssignment> DesksExamsAssignments { get; set; }

        [JsonProperty(PropertyName = "desksExtraAssignments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<DesksExtraAssignment> DesksExtraAssignments { get; set; }
    }
}
