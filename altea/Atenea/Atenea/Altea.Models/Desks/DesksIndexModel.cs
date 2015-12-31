namespace Altea.Models.Desks
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksIndexModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public bool Remote { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }

        [DataMember]
        public int CodeValidFor { get; set; }
    }
}
