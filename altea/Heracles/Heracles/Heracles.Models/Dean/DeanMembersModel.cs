namespace Heracles.Models.Dean
{
    using System.Collections.Generic;

    using Altea.Classes.Dean;

    using Newtonsoft.Json;

    public class DeanMembersModel
    {
        [JsonProperty(PropertyName = "users", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<DeanUser> Users { get; set; }

        [JsonProperty(PropertyName = "groups", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<DeanGroup> Groups { get; set; } 
    }
}
