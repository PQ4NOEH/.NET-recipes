namespace Altea.Common.Classes
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Level
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "languageFrom", Required = Required.Always)]
        public Language LanguageFrom { get; set; }

        [JsonProperty(PropertyName = "languageTo", Required = Required.Always)]
        public Language LanguageTo { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "userDisplayName", Required = Required.Always)]
        public string UserDisplayName { get; set; }

        [JsonProperty(PropertyName = "adminDisplayName", Required = Required.Always)]
        public string AdminDisplayName { get; set; }

        [JsonProperty(PropertyName = "isCategory", Required = Required.Always)]
        public bool IsCategory { get; set; }

        [JsonProperty(PropertyName = "selectable", Required = Required.Always)]
        public bool Selectable { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "hasUsers", Required = Required.Always)]
        public bool HasUsers { get; set; }

        [JsonProperty(PropertyName = "hasGroups", Required = Required.Always)]
        public bool HasGroups { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<Level> Children { get; set; }
    }
}
