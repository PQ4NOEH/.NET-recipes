using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;

namespace Davalor.PortalPaciente.Messages.Payable
{

    public partial class PayableAggregate : ISynchroAggregateRoot
    {
        public PayableAggregate()
        {
            PaymentTransaction = new HashSet<PaymentTransaction>();
        }

        public Guid Id { get; set; }

        public Guid CurrencyId { get; set; }

        public int PaymentStatus { get; set; }

        public float Price { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<PaymentTransaction> PaymentTransaction { get; set; }
    }
}
