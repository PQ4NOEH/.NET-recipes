using AlteaLabs.Common.Contracts;
using Newtonsoft.Json;
using System;

namespace AlteaLabs.WiseReader.Contracts
{
    public class BoardFile
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public FileType Type { get; set; }

        [JsonProperty(PropertyName = "uploaded", Required = Required.Always)]
        public bool Uploaded { get; set; }

        [JsonProperty(PropertyName = "language", Required = Required.Always)]
        public Language Language { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createDate", Required = Required.Always)]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate", Required = Required.Always)]
        public DateTime LastModifiedDate { get; set; }

        [JsonProperty(PropertyName = "processed", Required = Required.Always)]
        public bool Processed { get; set; }

        [JsonProperty(PropertyName = "converted", Required = Required.Always)]
        public bool Converted { get; set; }

        [JsonProperty(PropertyName = "opened", Required = Required.Always)]
        public bool Opened { get; set; }

        [JsonProperty(PropertyName = "invalid", Required = Required.Always)]
        public bool Invalid { get; set; }
    }
}
