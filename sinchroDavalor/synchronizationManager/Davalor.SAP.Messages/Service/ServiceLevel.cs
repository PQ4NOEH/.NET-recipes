using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Service
{
    public partial class ServiceLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceLevel()
        {
            ServicePrice = new List<ServicePrice>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string LongDescriptionKeyId { get; set; }

        public int Deleted { get; set; }

        public Guid ServiceId { get; set; }

        [Required]
        [StringLength(18)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ServiceAggregate Service { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<ServicePrice> ServicePrice { get; set; }
    }
}
