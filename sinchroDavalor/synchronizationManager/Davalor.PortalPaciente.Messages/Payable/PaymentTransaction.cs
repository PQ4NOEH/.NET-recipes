
using System;
using System.Collections.Generic;

namespace Davalor.PortalPaciente.Messages.Payable
{
    public partial class PaymentTransaction
    {
        public PaymentTransaction()
        {
            PaymentTaxTransaction = new HashSet<PaymentTaxTransaction>();
        }

        public Guid Id { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid PayableId { get; set; }

        public Guid PaymentId { get; set; }

        public Guid PartnerId { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual PayableAggregate Payable { get; set; }

        public virtual PaymentEntity Payment { get; set; }

        public virtual ICollection<PaymentTaxTransaction> PaymentTaxTransaction { get; set; }
    }
}
