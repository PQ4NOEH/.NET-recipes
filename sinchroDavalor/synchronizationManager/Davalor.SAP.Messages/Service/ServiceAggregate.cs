
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Service
{
    public partial class ServiceAggregate : ISynchroAggregateRoot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceAggregate()
        {
            ServiceLevel = new List<ServiceLevel>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string LongDescriptionKeyId { get; set; }

        public byte[] Cover { get; set; }

        [StringLength(5)]
        public string CoverType { get; set; }

        public int Deleted { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid DecisionTreeId { get; set; }

        [Required]
        [StringLength(18)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ServiceType ServiceType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<ServiceLevel> ServiceLevel { get; set; }
    }
}
