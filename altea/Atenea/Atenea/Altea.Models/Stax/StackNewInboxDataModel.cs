namespace Altea.Models.Stax
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Stax;
    using Altea.Common.Classes;

    [DataContract]
    public class StackNewInboxDataModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Language From { get; set; }

        [DataMember]
        public Language To { get; set; }

        [DataMember]
        public StackType Type { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public int Origin { get; set; }
        
        [DataMember]
        public int? Reference { get; set; }

        [DataMember]
        public bool Searched { get; set; }

        [DataMember]
        public string Sentence { get; set; }

        [DataMember]
        public int InboxOverflow { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
