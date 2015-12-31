namespace Heracles.Models.WiseLab
{
    using Altea.Classes.WiseLab;
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class WiseLabArticleModel
    {
        [JsonProperty(PropertyName = "article", Required = Required.Always)]
        public WiseLabArticle Article { get; set; }

        [JsonProperty(PropertyName = "parameters", Required = Required.Always)]
        public WiseLabPropertyAttribute Parameters { get; set; }
        
        [JsonProperty(PropertyName = "languageFrom", Required = Required.Always)]
        public Language LanguageFrom { get; set; }

        [JsonProperty(PropertyName = "languageTo", Required = Required.Always)]
        public Language LanguageTo { get; set; }
    }
}
