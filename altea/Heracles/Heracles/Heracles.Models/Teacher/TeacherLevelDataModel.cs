namespace Heracles.Models.Teacher
{
    using Altea.Classes.Desks;

    using Newtonsoft.Json;

    public class TeacherLevelDataModel
    {
        [JsonProperty(PropertyName = "index", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DesksIndexList Index { get; set; }

        [JsonProperty(PropertyName = "exams", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DesksExamList Exams { get; set; }

        [JsonProperty(PropertyName = "extra", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DesksExtraList Extra { get; set; }

    }
}