namespace Altea.Models.Desks
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksFinishExamModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
