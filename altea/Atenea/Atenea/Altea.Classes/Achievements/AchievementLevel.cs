namespace Altea.Classes.Achievements
{
    using Newtonsoft.Json;

    public class AchievementLevel
    {
        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "points", Required = Required.Always)]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull)]
        public string Image { get; set; }
    }
}
