namespace Altea.Models.Desks
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksFinishIndexModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public TimeSpan Time { get; set; } 

        [DataMember]
        public int OffsetDate { get; set; } 
    }
}
