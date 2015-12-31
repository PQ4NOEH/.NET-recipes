namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankCreateModel : WiseTankModel
    {
        [DataMember]
        public string Name { get; set; }
    }
}
