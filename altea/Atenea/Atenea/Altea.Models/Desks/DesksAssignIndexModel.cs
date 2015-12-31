namespace Altea.Models.Desks
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;

    [DataContract]
    public class DesksAssignIndexModel
    {
        [DataMember]
        public Guid Member { get; set; }

        [DataMember]
        public AlteaMemberType MemberType { get; set; }

        [DataMember]
        public DesksIndexQuestionType AssignmentType { get; set; }

        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public int Area { get; set; }
        
        [DataMember]
        public int Subject { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public int Num { get; set; }

        [DataMember]
        public bool Remote { get; set; }

        [DataMember]
        public bool Assign { get; set; }

        [DataMember]
        public bool Unblock { get; set; }

        [DataMember]
        public Guid AssignmentTeacher { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}