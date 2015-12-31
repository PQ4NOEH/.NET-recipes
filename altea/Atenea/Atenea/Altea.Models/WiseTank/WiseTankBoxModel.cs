namespace Altea.Models.WiseTank
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankBoxModel : WiseTankStreamModel
    {
        [DataMember]
        public Guid Box { get; set; }
    }
}
