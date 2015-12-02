
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Title
{
    public partial class TitleAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        public int Deleted { get; set; }

        [Required]
        [StringLength(4)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
