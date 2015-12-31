namespace Altea.Models.WiseNet
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class WiseNetCreateModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
