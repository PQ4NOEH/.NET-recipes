namespace Altea.Common.Classes
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class SpeechType
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "default", Required = Required.Always)]
        public bool Default { get; set; }

        [JsonProperty(PropertyName = "selectable", Required = Required.Always)]
        public bool Selectable { get; set; }

        [JsonProperty(PropertyName = "image", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "voices", Required = Required.Always)]
        public IEnumerable<SpeechVoice> Voices { get; set; } 
    }
}
