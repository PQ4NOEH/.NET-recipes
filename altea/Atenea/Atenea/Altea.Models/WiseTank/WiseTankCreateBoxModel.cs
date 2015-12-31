namespace Altea.Models.WiseTank
{
    using System;

    using System.Runtime.Serialization;

    using Altea.Classes.WiseTank;

    [DataContract]
    public class WiseTankCreateBoxModel : WiseTankCreateModel
    {
        [DataMember]
        public Guid Stream { get; set; }

        [DataMember]
        public TankBoxType BoxType { get; set; }

        [DataMember]
        public string Query { get; set; }
    }
}
