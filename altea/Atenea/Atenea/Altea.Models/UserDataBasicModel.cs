namespace Altea.Models
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class UserDataBasicModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public Language LanguageFrom { get; set; }

        [DataMember]
        public Language LanguageTo { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
