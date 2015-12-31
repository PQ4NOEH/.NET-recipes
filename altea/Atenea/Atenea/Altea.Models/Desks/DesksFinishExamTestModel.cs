namespace Altea.Models.Desks
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksFinishExamTestModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
