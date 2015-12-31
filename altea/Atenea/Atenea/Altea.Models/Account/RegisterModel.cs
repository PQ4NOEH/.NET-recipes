namespace Altea.Models.Account
{
    using System.Runtime.Serialization;

    [DataContract]
    public class RegisterModel
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }
}
