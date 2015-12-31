namespace Altea.Models.Stax
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Altea.Classes.Stax;
    using Altea.Common.Classes;

    [DataContract]
    public class StackAcceptInboxDataModel
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
        public long Id { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public int MaxData { get; set; }

        [DataMember]
        public IEnumerable<StaxContentData> DataOptions { get; set; }
        
        [DataMember]
        public int OffsetDate { get; set; }
    }
}
