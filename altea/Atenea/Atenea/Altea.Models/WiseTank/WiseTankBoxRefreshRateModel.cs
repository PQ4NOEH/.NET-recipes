namespace Altea.Models.WiseTank
{
    using System;

    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankBoxRefreshRateModel : WiseTankModel
    {

        [DataMember]
        public Guid Stream { get; set; }

        [DataMember]
        public int RefreshRate { get; set; }
    }
}
