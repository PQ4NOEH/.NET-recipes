namespace Heracles.Models.WiseReader
{
    using System;

    using Newtonsoft.Json;

    public class WiseReaderFileViewerModel
    {
        [JsonProperty(PropertyName = "fileId", Required = Required.Always)]
        public Guid FileId { get; set; }

        [JsonProperty(PropertyName = "fileName", Required = Required.Always)]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "opened", Required = Required.Always)]
        public bool Opened { get; set; }

        [JsonProperty(PropertyName = "converted", Required = Required.Always)]
        public bool Converted { get; set; }

        [JsonProperty(PropertyName = "folderId", Required = Required.Always)]
        public Guid FolderId { get; set; }

        [JsonProperty(PropertyName = "folderLevel", Required = Required.Always)]
        public int FolderLevel { get; set; }

        [JsonProperty(PropertyName = "folderName", Required = Required.Always)]
        public string FolderName { get; set; }

        [JsonProperty(PropertyName = "referenceId", Required = Required.Always)]
        public int ReferenceId { get; set; }
    }
}
