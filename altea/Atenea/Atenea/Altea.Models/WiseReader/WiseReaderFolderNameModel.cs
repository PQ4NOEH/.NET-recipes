namespace Altea.Models.WiseReader
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseReaderFolderNameModel : WiseReaderFolderModel
    {

        [DataMember]
        public string Name { get; set; }
    }
}
