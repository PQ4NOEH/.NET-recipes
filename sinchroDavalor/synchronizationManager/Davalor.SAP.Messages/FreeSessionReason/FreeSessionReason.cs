using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.FreeSessionReason
{
    public partial class FreeSessionReasonAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(3)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
