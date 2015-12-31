namespace Altea.Models.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Altea.Classes.Admin;
    using Altea.Common.Classes;

    [DataContract]
    public class AdminSetMemberLevelsModel
    {
        [DataMember]
        public Guid Member { get; set; }

        [DataMember]
        public Guid Application { get; set; }

        [DataMember]
        public Language LanguageFrom { get; set; }

        [DataMember]
        public Language LanguageTo { get; set; }
    
        [DataMember]
        public IEnumerable<AdminMemberLevel> Levels { get; set; } 
    }
}
