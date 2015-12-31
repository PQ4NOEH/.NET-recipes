namespace Altea.Classes.Achievements
{
    using System;

    using Newtonsoft.Json;

    public class UserAchievement
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Achievement { get; set; }

        [JsonProperty(PropertyName = "level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "unlockDate", Required = Required.Always)]
        public DateTime UnlockDate { get; set; }
    }
}
