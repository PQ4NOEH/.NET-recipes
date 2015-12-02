using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;

namespace Davalor.PortalPaciente.Messages.Answer
{
    public partial class AnswerAggregate : ISynchroAggregateRoot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AnswerAggregate()
        {
            AnswerValues = new HashSet<AnswerValues>();
        }

        public Guid Id { get; set; }

        public Guid PersonId { get; set; }

        public Guid? SessionId { get; set; }

        public Guid? AppointmentId { get; set; }

        public int Type { get; set; }

        public Guid QuestionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnswerValues> AnswerValues { get; set; }
    }
}
