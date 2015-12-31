namespace Altea.Models.Stax
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Stax;
    using Altea.Common.Classes;

    [DataContract]
    public class StaxCheckModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Language LanguageFrom { get; set; }

        [DataMember]
        public Language LanguageTo { get; set; }

        [DataMember]
        public StackType Type { get; set; }

        [DataMember]
        public int MaxStack { get; set; }
    }
}
