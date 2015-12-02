using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Service
{
    public partial class ServicePrice
    {
        public Guid Id { get; set; }

        public DateTime BeginPeriod { get; set; }

        public DateTime EndPeriod { get; set; }

        public float Price { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid ServiceLevelId { get; set; }

        public Guid CountryId { get; set; }

        [Required]
        [StringLength(1)]
        public string TaxClass { get; set; }

        [Required]
        [StringLength(100)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ServiceLevel ServiceLevel { get; set; }
    }
}
