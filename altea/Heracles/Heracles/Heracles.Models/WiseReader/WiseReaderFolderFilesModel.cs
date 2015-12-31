namespace Heracles.Models.WiseReader
{
    using System.Collections.Generic;

    using Altea.Classes.WiseReader;

    using Newtonsoft.Json;

    public class WiseReaderFolderFilesModel
    {
        [JsonProperty(PropertyName = "fileIds", Required = Required.Always)]
        public IDictionary<string, string> FileIds { get; set; }
        
        [JsonProperty(PropertyName = "fileTypes", Required = Required.Always)]
        public IDictionary<string, FileType> FileTypes { get; set; }

        [JsonProperty(PropertyName = "processedFiles", Required = Required.Always)]
        public IDictionary<string, string> ProcessedFiles { get; set; } 
    }
}
