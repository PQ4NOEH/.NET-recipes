namespace Altea.Models.WiseTank
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankStreamModel : WiseTankModel
    {
        [DataMember]
        public Guid Stream { get; set; }
    }
}
