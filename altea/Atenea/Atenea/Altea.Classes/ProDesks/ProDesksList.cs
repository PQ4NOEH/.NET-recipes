namespace Altea.Classes.ProDesks
{
    using Altea.Classes.Desks;

    using Newtonsoft.Json;

    public class ProDesksList
    {
        [JsonProperty(PropertyName = "index", Required = Required.Always)]
        public DesksIndexList Index { get; set; }

        [JsonProperty(PropertyName = "exams", Required = Required.Always)]
        public DesksExamList Exams { get; set; }
    }
}