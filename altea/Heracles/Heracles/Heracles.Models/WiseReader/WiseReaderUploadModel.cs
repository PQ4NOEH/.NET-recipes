namespace Heracles.Models.WiseReader
{
    using System.Collections.Generic;

    using Altea.Classes.WiseReader;

    using Newtonsoft.Json;

    public class WiseReaderUploadModel : WiseReaderFolderFilesModel
    {
        [JsonProperty(PropertyName = "uploadStatus", Required = Required.Always)]
        public IDictionary<string, FileUploadStatus> UploadStatus { get; set; }
    }
}
