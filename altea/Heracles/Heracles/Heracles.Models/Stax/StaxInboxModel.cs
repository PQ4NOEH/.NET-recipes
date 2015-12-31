namespace Heracles.Models.Stax
{
    using System.Collections.Generic;

    using Altea.Classes.Stax;

    using Newtonsoft.Json;

    public class StaxInboxModel
    {
        [JsonProperty(PropertyName = "inbox", Required = Required.Always)]
        public IEnumerable<IStackInboxData> Inbox { get; set; }

        [JsonProperty(PropertyName = "dataInStax", Required = Required.Always)]
        public IDictionary<string, IEnumerable<string>> DataInStax { get; set; }
    }
}
