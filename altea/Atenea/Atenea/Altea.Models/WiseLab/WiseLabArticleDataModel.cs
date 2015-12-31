namespace Altea.Models.WiseLab
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseLab;
    using Altea.Common.Classes;

    [DataContract]
    public class WiseLabArticleDataModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Language LanguageFrom { get; set; }

        [DataMember]
        public Language LanguageTo { get; set; }

        [DataMember]
        public WiseLabOrigin Origin { get; set; }

        [DataMember]
        public int Reference { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
