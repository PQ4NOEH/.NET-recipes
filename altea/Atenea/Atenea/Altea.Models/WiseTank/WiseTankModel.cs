namespace Altea.Models.WiseTank
{
    using System;

    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class WiseTankModel
    {
        [DataMember]
        public Guid User { get; set; }

        [DataMember]
        public Guid App { get; set; }
        
        [DataMember]
        public Language Language { get; set; }
    }
}
