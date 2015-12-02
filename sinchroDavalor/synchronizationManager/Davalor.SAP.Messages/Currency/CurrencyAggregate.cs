
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Currency
{
    public partial class CurrencyAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(3)]
        public string IsoCodeChar { get; set; }

        [Required]
        [StringLength(3)]
        public string IsoCodeNum { get; set; }

        public int Decimals { get; set; }

        [Required]
        [StringLength(5)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
