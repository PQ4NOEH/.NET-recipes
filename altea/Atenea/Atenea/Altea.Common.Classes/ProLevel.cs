namespace Altea.Common.Classes
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ProLevel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "subId", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? SubId { get; set; }

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

        [JsonProperty(PropertyName = "subName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string SubName { get; set; }

        [JsonProperty(PropertyName = "userDisplaySubName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string UserDisplaySubName { get; set; }

        [JsonProperty(PropertyName = "adminDisplaySubName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string AdminDisplaySubName { get; set; }

        [JsonProperty(PropertyName = "isCategory", Required = Required.Always)]
        public bool IsCategory { get; set; }

        [JsonProperty(PropertyName = "selectable", Required = Required.Always)]
        public bool Selectable { get; set; }

        [JsonProperty(PropertyName = "forceAcademicLevel", Required = Required.Always)]
        public bool ForceAcademicLevel { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "hasUsers", Required = Required.Always)]
        public bool HasUsers { get; set; }

        [JsonProperty(PropertyName = "hasGroups", Required = Required.Always)]
        public bool HasGroups { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<ProLevel> Children { get; set; }

        [JsonProperty(PropertyName = "subLevels", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<ProLevel> SubLevels { get; set; }
    }
}
