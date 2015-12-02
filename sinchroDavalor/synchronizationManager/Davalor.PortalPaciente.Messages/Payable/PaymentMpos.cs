
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.Payable
{

    public partial class PaymentMpos
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string BankingNumber { get; set; }

        [StringLength(50)]
        public string AuthNumber { get; set; }

        [StringLength(50)]
        public string OperationNumber { get; set; }

        [StringLength(8)]
        public string Sequence { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual PaymentEntity Payment { get; set; }
    }
}
