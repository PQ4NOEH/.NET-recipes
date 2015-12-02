using Davalor.SynchronizationManager.Domain.Repository;
using System;

namespace Davalor.PortalPaciente.Messages.DisclaimerSignature
{
    public partial class DisclaimerSignatureAggregate : ISynchroAggregateRoot
    {
        public DisclaimerSignatureAggregate()
        {
        }

        public Guid Id { get; set; }

        public Int32 State { get; set; }

        public DateTimeOffset SignedDate { get; set; }

        public Guid SignatureId { get; set; }

        public Guid PatientId { get; set; }

        public Guid DisclaimerId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public Signature Signature { get; set; }
    }
}
