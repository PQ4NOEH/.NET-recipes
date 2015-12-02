

using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Country 
{

    public partial class CountryAggregate: ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public Guid CurrencyId { get; set; }

        [Required]
        [StringLength(2)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
