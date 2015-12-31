namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksIndexList
    {
        [JsonProperty(PropertyName = "types", Required = Required.Always)]
        public IEnumerable<DesksIndexType> Types;

        [JsonProperty(PropertyName = "subjects", Required = Required.Always)]
        public IEnumerable<DesksIndexSubject> Subjects;
    }
}
