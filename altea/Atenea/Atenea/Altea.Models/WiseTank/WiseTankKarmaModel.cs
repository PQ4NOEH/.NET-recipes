namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankKarmaModel : WiseTankModel
    {
        [DataMember]
        public long Article { get; set; }
        
        [DataMember]
        public int? Karma { get; set; }
    }
}
