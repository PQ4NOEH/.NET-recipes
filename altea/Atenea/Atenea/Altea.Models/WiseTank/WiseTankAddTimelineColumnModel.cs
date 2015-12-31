namespace Altea.Models.WiseTank
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;

    [DataContract]
    public class WiseTankAddTimelineColumnModel : WiseTankModel
    {
        [DataMember]
        public Guid Timeline { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int ThinkTankLevel { get; set; }

        [DataMember]
        public TankAccessType AccessType { get; set; }

        [DataMember]
        public bool ModeratedArticles { get; set; }

        [DataMember]
        public bool ModeratedComments { get; set; }

        [DataMember]
        public bool WriteOwnArticles { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
