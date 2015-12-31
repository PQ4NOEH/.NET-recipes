namespace Altea.Models.WiseTank
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;
    using Altea.Common.Classes;

    [DataContract]
    public class WiseTankCreateArticleModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public Guid Timeline { get; set; }

        [DataMember]
        public TankOrigin Origin { get; set; }

        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string Favicon { get; set; }
    
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Lead { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}