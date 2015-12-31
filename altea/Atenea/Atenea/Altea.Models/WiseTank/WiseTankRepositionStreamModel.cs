namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankRepositionStreamModel : WiseTankStreamModel
    {

        [DataMember]
        public int Position { get; set; }
    }
}
