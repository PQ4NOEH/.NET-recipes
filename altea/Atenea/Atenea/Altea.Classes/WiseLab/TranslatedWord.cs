namespace Altea.Classes.WiseLab
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TranslatedWord
    {
        [JsonProperty(PropertyName = "word", Required = Required.Always)]
        public string Word { get; set; }

        [JsonProperty(PropertyName = "suggestedTranslation", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string SuggestedTranslation { get; set; }

        [JsonProperty(PropertyName = "otherTranslations", Required = Required.Always)]
        public IEnumerable<Translation> OtherTranslations { get; set; } 
    }
}
