namespace Altea.Models.WiseReader
{
    using System;

    using System.Runtime.Serialization;

    using Altea.Classes.WiseReader;
    using Altea.Common.Classes;

    [DataContract]
    public class WiseReaderReferenceFileModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid FolderId { get; set; }

        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public string FileId { get; set; }

        [DataMember]
        public FileType FileType { get; set; }

        [DataMember]
        public string FileName { get; set; }
    }
}
