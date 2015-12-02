
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.Payable
{
    public partial class PaymentTaxTransaction 
    {
        public Guid Id { get; set; }

        public Guid PaymentTransactionId { get; set; }

        public Guid TaxId { get; set; }

        public decimal Amount { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual PaymentTransaction PaymentTransaction { get; set; }
    }
}
