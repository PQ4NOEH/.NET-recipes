namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankRepositionBoxModel : WiseTankBoxModel
    {
        [DataMember]
        public int Position { get; set; }
    }
}
