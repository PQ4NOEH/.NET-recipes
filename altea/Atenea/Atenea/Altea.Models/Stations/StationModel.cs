namespace Altea.Models.Stations
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Stations;

    [DataContract]
    public class StationModel
    {
        [DataMember]
        public Guid Application { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StationType Type { get; set; }

        [DataMember]
        public int Stations { get; set; }

        [DataMember]
        public int Position { get; set; }
    }
}
