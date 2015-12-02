using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Service
{
    public partial class ServiceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceType()
        {
            Service = new List<ServiceAggregate>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string LongDescriptionKeyId { get; set; }

        public int Length { get; set; }

        public int? Type { get; set; }

        public int Deleted { get; set; }

        [Required]
        [StringLength(20)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<ServiceAggregate> Service { get; set; }
    }
}
