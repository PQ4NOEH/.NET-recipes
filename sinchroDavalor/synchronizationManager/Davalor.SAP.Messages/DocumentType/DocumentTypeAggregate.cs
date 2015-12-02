using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.DocumentType
{
    public partial class DocumentTypeAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [StringLength(500)]
        public string NameKeyId { get; set; }

        public Guid CountryId { get; set; }

        [Required]
        [StringLength(2)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
