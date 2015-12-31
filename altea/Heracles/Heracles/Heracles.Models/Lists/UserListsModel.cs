namespace Heracles.Models.Lists
{
    using System.Collections.Generic;

    using Altea.Classes.Lists;

    using Newtonsoft.Json;

    public class UserListsModel
    {
        [JsonProperty(PropertyName = "groups", Required = Required.Always)]
        public IEnumerable<ListGroupType> Groups { get; set; }

        [JsonProperty(PropertyName = "lists", Required = Required.Always)]
        public IEnumerable<AssignedList> Lists { get; set; }  
    }
}
