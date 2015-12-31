namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankVoteModel : WiseTankModel
    {
        [DataMember]
        public long Article { get; set; }
        
        [DataMember]
        public bool? UpVote { get; set; }
    }
}
