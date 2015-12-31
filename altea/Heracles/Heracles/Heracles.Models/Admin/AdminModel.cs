namespace Heracles.Models.Admin
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AdminModel
    {
        [JsonProperty(PropertyName = "roles", Required = Required.Always)]
        public IEnumerable<string> Roles { get; set; }

        [JsonProperty(PropertyName = "modules", Required = Required.Always)]
        public IDictionary<string, IDictionary<string, IEnumerable<string>>> Modules { get; set; }

        [JsonProperty(PropertyName = "languages", Required = Required.Always)]
        public IDictionary<int, IEnumerable<int>> Languages { get; set; } 
    }
}
