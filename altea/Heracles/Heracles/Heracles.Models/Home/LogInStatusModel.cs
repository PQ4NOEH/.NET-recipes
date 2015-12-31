using Newtonsoft.Json;

namespace Heracles.Models.Home
{
    public class LogInStatusModel
    {
        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public bool Status { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
