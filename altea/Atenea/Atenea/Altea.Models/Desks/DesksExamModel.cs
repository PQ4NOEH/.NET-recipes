namespace Altea.Models.Desks
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksExamModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int CodeValidFor { get; set; }
    }
}
