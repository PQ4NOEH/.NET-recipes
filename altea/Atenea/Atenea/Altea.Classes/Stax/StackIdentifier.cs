namespace Altea.Classes.Stax
{
    using Altea.Classes.Members;
    using Altea.Common.Classes;

    using Newtonsoft.Json;

    public class StackIdentifier
    {
        [JsonProperty(PropertyName = "user", Required = Required.Always)]
        public User User { get; set; }

        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public Language From { get; set; }

        [JsonProperty(PropertyName = "to", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Language To { get; set; }
    }
}
