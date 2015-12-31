namespace Altea.Classes.Group
{
    using Newtonsoft.Json;

    public class AlteaGroupPlanning
    {
        [JsonProperty(PropertyName = "finished", Required = Required.Always)]
        public bool Finished { get; set; }
    }
}
