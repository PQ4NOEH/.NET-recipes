namespace Altea.Models.WiseReader
{
    using System;

    using System.Runtime.Serialization;

    using Altea.Classes.WiseReader;
    using Altea.Common.Classes;

    [DataContract]
    public class WiseReaderQueueProcessFileModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public byte[] Checksum { get; set; }

        [DataMember]
        public FileType Type { get; set; }

        [DataMember]
        public Guid Uploader { get; set; }
    }
}
