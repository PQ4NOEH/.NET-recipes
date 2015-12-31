namespace Heracles.Models.Dean
{
    using System.Collections.Generic;

    using Altea.Classes.Lists;

    using Newtonsoft.Json;

    public class DeanListsModel
    {
        [JsonProperty(PropertyName = "categories", Required = Required.Always)]
        public IEnumerable<ListCategory> Categories { get; set; }

        [JsonProperty(PropertyName = "lists", Required = Required.Always)]
        public IEnumerable<AlteaList> Lists { get; set; }
    }
}
