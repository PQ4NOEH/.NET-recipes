namespace Altea.Models.WiseLab
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseLabHuntDataModel : WiseLabArticleDataModel
    {
        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public string Sentence { get; set; }

        [DataMember]
        public int InboxOverflow { get; set; }
    }
}
