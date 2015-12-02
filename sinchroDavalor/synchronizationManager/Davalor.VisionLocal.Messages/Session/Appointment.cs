using System;

namespace Davalor.VisionLocal.Messages.Session
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public DateTime InitialTime { get; set; }

        public DateTime FinalTime { get; set; }

        public int StatusType { get; set; }

        public Guid TimeZoneId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid ServiceLevelId { get; set; }

        public Guid ServiceId { get; set; }

        public Guid MediaId { get; set; }

        public Guid PatientId { get; set; }

        public Guid MachineId { get; set; }

        public Guid PayableId { get; set; }

        public Guid? SessionId { get; set; }

        public Guid PartnerId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual SessionAggregate Session { get; set; }
    }
}
