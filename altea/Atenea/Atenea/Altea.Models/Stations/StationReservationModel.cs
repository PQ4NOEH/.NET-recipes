namespace Altea.Models.Stations
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Stations;

    [DataContract]
    public class StationReservationModel
    {
        [DataMember]
        public StationType Type { get; set; }

        [DataMember]
        public StationFrequency Frequency { get; set; }

        [DataMember]
        public int Stations { get; set; }

        [DataMember]
        public DayOfWeek WeekDay { get; set; }

        [DataMember]
        public int Hour { get; set; }

        [DataMember]
        public int Minute { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
