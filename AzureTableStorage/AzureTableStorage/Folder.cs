using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStorage
{
    public class Folder
    {
        public List<Folder> SubFolders { get; set; }
    }

    public class FileMetadata
    {
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string BlobReferenceKey { get; set; }
    }
}
