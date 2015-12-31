namespace Heracles.Models.WiseReader
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.WiseReader;

    using Newtonsoft.Json;

    public class WiseReaderFoldersModel
    {
        [JsonProperty(PropertyName = "folderIds", Required = Required.Always)]
        public IEnumerable<Guid> FolderIds { get; set; } 

        [JsonProperty(PropertyName = "rootFolder", Required = Required.Always)]
        public Folder RootFolder { get; set; }
    }
}
