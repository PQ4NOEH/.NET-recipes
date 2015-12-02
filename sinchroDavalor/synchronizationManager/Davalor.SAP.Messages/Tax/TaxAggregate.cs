using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Tax
{
    public partial class TaxAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid CountryId { get; set; }

        public Guid CurrencyId { get; set; }

        [Required]
        [StringLength(1)]
        public string TaxClassPartner { get; set; }

        [Required]
        [StringLength(1)]
        public string TaxClassServicePrice { get; set; }

        public DateTimeOffset BeginPeriod { get; set; }

        public DateTimeOffset EndPeriod { get; set; }

        [Required]
        [StringLength(1)]
        public string Rule { get; set; }

        public decimal? Amount { get; set; }

        public decimal? BaseAmount { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(12)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
