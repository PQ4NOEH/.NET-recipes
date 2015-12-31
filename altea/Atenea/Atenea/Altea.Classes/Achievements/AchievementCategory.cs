namespace Altea.Classes.Achievements
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AchievementCategory
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<Achievement> Achievements { get; set; }
    }
}
