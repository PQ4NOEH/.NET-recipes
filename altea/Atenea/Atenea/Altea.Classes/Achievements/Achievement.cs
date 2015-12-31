namespace Altea.Classes.Achievements
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Achievement
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "secret", Required = Required.Always)]
        public bool Secret { get; set; }

        [JsonProperty(PropertyName = "hidden", Required = Required.Always)]
        public bool Hidden { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<AchievementLevel> Levels { get; set; }
   } 
}
