namespace Altea.Models.Desks
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksAutoUnblockModel
    {
        [DataMember]
        public Guid User { get; set; }

        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public int Days { get; set; }
    }
}