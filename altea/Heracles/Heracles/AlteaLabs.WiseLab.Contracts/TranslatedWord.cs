using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.WiseLab.Contracts
{
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
