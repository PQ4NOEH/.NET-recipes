using AlteaLabs.WiseReader.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AlteaLabs.WiseReader.Models
{
    public class FoldersModel
    {
        [JsonProperty(PropertyName = "folderIds", Required = Required.Always)]
        public List<Guid> FolderIds { get; set; }

        [JsonProperty(PropertyName = "rootFolder", Required = Required.Always)]
        public Folder RootFolder { get; set; }
    }
}
