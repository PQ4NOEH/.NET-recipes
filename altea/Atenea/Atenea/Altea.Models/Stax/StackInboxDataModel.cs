namespace Altea.Models.Stax
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Stax;
    using Altea.Common.Classes;

    [DataContract]
    public class StackInboxDataModel
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
        public int Id { get; set; }
    }
}
