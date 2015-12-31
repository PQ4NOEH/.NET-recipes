namespace Heracles.Models.WiseTank
{
    using System.Collections.Generic;

    using Altea.Classes.WiseTank;

    using Newtonsoft.Json;

    public class WiseTankStreamBoxDataModel
    {
        [JsonProperty(PropertyName = "articles", Required = Required.Always)]
        public IEnumerable<TankStreamArticle> Articles { get; set; }

        [JsonProperty(PropertyName = "articleCount", Required = Required.Always)]
        public int ArticleCount { get; set; }
    }
}
