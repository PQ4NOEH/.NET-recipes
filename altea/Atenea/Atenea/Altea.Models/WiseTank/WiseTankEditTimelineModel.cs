namespace Altea.Models.WiseTank
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;

    [DataContract]
    public class WiseTankEditTimelineModel : WiseTankModel
    {
        [DataMember]
        public Guid Timeline { get; set; }

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
    }
}
