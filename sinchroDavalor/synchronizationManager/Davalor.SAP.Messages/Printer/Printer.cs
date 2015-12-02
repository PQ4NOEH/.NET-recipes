
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Printer
{
    public partial class PrinterAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Code { get; set; }

        public Guid? PartnerId { get; set; }

        public bool POSPrint { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
