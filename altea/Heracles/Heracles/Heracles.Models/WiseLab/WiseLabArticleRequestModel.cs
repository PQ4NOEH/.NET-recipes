namespace Heracles.Models.WiseLab
{
    using Altea.Classes.WiseLab;

    using Newtonsoft.Json;

    public class WiseLabArticleRequestModel
    {
        [JsonProperty(PropertyName = "article", Required = Required.Always)]
        public WiseLabArticle Article { get; set; }

        [JsonProperty(PropertyName = "parameters", Required = Required.Always)]
        public WiseLabPropertyAttribute Parameters { get; set; }
    }
}
