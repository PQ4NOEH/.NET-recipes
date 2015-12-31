namespace Altea.Models.WiseTank
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;

    [DataContract]
    public class WiseTankSetTimelineAreaModel : WiseTankModel
    {
        [DataMember]
        public Guid Timeline { get; set; }

        [DataMember]
        public TankArea Area { get; set; }
    }
}
