
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Region
{
    public partial class RegionAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public Guid CountryId { get; set; }

        [Required]
        [StringLength(5)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
