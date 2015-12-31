namespace Altea.Models.WiseTank
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WiseTankEditStreamModel : WiseTankStreamModel
    {

        [DataMember]
        public string Name { get; set; }
    }
}
