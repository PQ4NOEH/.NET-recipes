namespace Altea.Models.Dictionary
{
    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class DictionaryInsertSpeechModel
    {
        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public string Word { get; set; }

        [DataMember]
        public byte[] Audio { get; set; }
    }
}
