namespace Altea.Classes.WiseReader
{
    public class BoardStorageFile
    {
        public string Name { get; set; }
        public string StorageName { get; set; }
        public FileType Type { get; set; }
        public bool Uploaded { get; set; }
        public bool Converted { get; set; }
        public bool Opened { get; set; }
    }
}