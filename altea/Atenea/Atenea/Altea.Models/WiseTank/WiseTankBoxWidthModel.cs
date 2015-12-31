namespace Altea.Models.WiseTank
{
    using System;

    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankBoxWidthModel : WiseTankModel
    {

        [DataMember]
        public Guid Data { get; set; }

        [DataMember]
        public int Width { get; set; }
    }
}
