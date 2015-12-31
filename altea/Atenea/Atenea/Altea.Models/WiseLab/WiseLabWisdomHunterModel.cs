namespace Altea.Models.WiseLab
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseLabWisdomHunterModel : WiseLabArticleDataModel
    {
        [DataMember]
        public string Lead { get; set; }

        [DataMember]
        public bool AutoSave { get; set; }
    }
}
