namespace Altea.Models.WiseTank
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;

    [DataContract]
    public class WiseTankCreateTimelineModel : WiseTankModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public TankAccessType AccessType { get; set; }

        [DataMember]
        public bool ModeratedArticles { get; set; }

        [DataMember]
        public bool ModeratedComments { get; set; }

        [DataMember]
        public bool WriteOwnArticles { get; set; }

        [DataMember]
        public IDictionary<int, int> PermissionTypes { get; set; }

        [DataMember]
        public TankArea Area { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
