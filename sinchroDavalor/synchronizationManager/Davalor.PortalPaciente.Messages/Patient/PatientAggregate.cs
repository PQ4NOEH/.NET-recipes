
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Davalor.PortalPaciente.Messages.Patient
{
    public partial class PatientAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid PersonId { get; set; }

        public Guid? UserId { get; set; }

        [StringLength(10)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual Person Person { get; set; }

        public virtual User User { get; set; }
    }
}
