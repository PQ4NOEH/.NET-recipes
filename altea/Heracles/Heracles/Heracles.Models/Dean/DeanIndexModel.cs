namespace Heracles.Models.Dean
{
    using System.Collections.Generic;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class DeanIndexModel
    {
        [JsonProperty(PropertyName = "sticky", Required = Required.Always)]
        public DeanIndexStickyModel Sticky { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<Level> Levels { get; set; }

        [JsonProperty(PropertyName = "proLevels", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ProLevel> ProLevels { get; set; }

        [JsonProperty(PropertyName = "columns", Required = Required.Always)]
        public dynamic Columns { get; set; }

        [JsonProperty(PropertyName = "indexAreas", Required = Required.Always)]
        public IEnumerable<DesksIndexArea> IndexAreas { get; set; }

        [JsonProperty(PropertyName = "booksStudentColumns", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public dynamic BooksStudentColumns { get; set; }

        [JsonProperty(PropertyName = "extraTeacherColumns", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public dynamic ExtraTeacherColumns { get; set; }

        [JsonProperty(PropertyName = "proColumns", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public dynamic ProColumns { get; set; }
    }
}
