namespace Altea.Classes.WiseNet
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class WiseNetSearchEngine
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "searchUrl", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string SearchUrl { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "visible", Required = Required.Always)]
        public bool Visible { get; set; }

        [JsonProperty(PropertyName = "default", Required = Required.Always)]
        public bool Default { get; set; }

        [JsonProperty(PropertyName = "sections", Required = Required.Always)]
        public ICollection<WiseNetSearchEngineSection> Sections { get; set; } 
    }
}
