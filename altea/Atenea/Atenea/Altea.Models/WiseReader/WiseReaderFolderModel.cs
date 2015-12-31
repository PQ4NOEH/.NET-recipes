namespace Altea.Models.WiseReader
{
    using System;

    using System.Runtime.Serialization;

    [DataContract]
    public class WiseReaderFolderModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid Folder { get; set; }
    }
}
