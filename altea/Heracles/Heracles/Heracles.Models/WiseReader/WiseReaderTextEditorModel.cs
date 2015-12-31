namespace Heracles.Models.WiseReader
{
    using System;

    using Newtonsoft.Json;

    public class WiseReaderTextEditorModel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }
    }
}
