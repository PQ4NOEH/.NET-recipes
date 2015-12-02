
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Partner
{
    public partial class PartnerAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(241)]
        public string Email { get; set; }

        [StringLength(2048)]
        public string Url { get; set; }

        public int Deleted { get; set; }

        public Guid TimeZoneId { get; set; }

        public Guid LanguageId { get; set; }

        public Guid LocationId { get; set; }

        public Guid? PartnerChainId { get; set; }

        public Guid? CurrencyId { get; set; }

        [Required]
        [StringLength(1)]
        public string TaxClass { get; set; }

        [Required]
        [StringLength(30)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }


        public virtual PartnerChain PartnerChain { get; set; }
    }
}
