namespace Altea.Models.Desks
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class DesksCheckIndexModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public IEnumerable<int> Questions { get; set; }

        [DataMember]
        public IEnumerable<IEnumerable<string>> Answers { get; set; }

        [DataMember]
        public TimeSpan Time { get; set; } 
    }
}
