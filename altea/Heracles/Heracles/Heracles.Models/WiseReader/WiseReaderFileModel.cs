namespace Heracles.Models.WiseReader
{
    using System;

    using Newtonsoft.Json;

    public class WiseReaderFileModel
    {
        [JsonProperty(PropertyName = "fileId", Required = Required.Always)]
        public Guid FileId { get; set; }

        [JsonProperty(PropertyName = "folderId", Required = Required.Always)]
        public Guid FolderId { get; set; }
    }
}
