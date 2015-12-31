namespace Altea.Classes.WiseNet
{
    using Newtonsoft.Json;

    public class WiseNetSearchEngineSection
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "visible", Required = Required.Always)]
        public bool Visible { get; set; }
    }
}
