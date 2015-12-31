namespace Altea.Models.WiseTank
{
    using System;

    using System.Runtime.Serialization;

    using Altea.Common.Classes;

    [DataContract]
    public class WiseTankAddTimelineUserModel
    {
        [DataMember]
        public Guid User { get; set; }

        [DataMember]
        public Guid App { get; set; }

        [DataMember]
        public Language Language { get; set; }

        [DataMember]
        public Guid Timeline { get; set; }

        [DataMember]
        public Guid NewUser { get; set; }

        [DataMember]
        public int ThinkTankLevel { get; set; }

        [DataMember]
        public int Role { get; set; }

        [DataMember]
        public int Permissions { get; set; }
    }
}
