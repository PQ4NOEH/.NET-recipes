namespace Altea.Models.User
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class CreateUserModel
    {
        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public Language LanguageFrom { get; set; }

        [DataMember]
        public Language LanguageTo { get; set; }
    }
}
