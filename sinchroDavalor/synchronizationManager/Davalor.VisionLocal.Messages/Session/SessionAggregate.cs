
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;

namespace Davalor.VisionLocal.Messages.Session
{
    public class SessionAggregate : ISynchroAggregateRoot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SessionAggregate()
        {
            Appointment = new HashSet<Appointment>();
            Diagnosis = new HashSet<Diagnosis>();
            SessionDevice = new HashSet<SessionDevice>();
        }

        public Guid Id { get; set; }

        public DateTimeOffset? InitialTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public Guid? InvoiceId { get; set; }

        public int InvoiceRequired { get; set; }

        public bool InterpretationDone { get; set; }

        public DateTimeOffset? SignedDate { get; set; }

        public Guid? GuardianId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PatientId { get; set; }

        public Guid ServiceLevelId { get; set; }

        public Guid ServiceId { get; set; }

        public Guid MediaId { get; set; }

        public Guid MachineId { get; set; }

        public Guid? PayableId { get; set; }

        public Guid? CommissionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Diagnosis> Diagnosis { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SessionDevice> SessionDevice { get; set; }

    }
}
