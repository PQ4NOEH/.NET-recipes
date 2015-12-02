
using System;
using System.Collections.Generic;

namespace Davalor.PortalPaciente.Messages.Payable
{

    public partial class PaymentEntity
    {
        public PaymentEntity()
        {
            PaymentTransaction = new HashSet<PaymentTransaction>();
        }

        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime? RequestDate { get; set; }

        public int OperationType { get; set; }

        public int ProcessingState { get; set; }

        public int PaymentType { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid GatewayId { get; set; }

        public Guid PatientId { get; set; }

        public Guid? FreeSessionReasonId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual PaymentMpos PaymentMpos { get; set; }

        public virtual ICollection<PaymentTransaction> PaymentTransaction { get; set; }
    }
}
