
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Media
{
    public partial class MediaAggregate : ISynchroAggregateRoot
    {
        public MediaAggregate()
        {
            MediaDeviceGroup = new HashSet<MediaDeviceGroup>();
            MediaMachine = new HashSet<MediaMachine>();
            MediaServiceLevel = new HashSet<MediaServiceLevel>();
        }

        public Guid Id { get; set; }

        [StringLength(30)]
        public string ShortName { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string LongDescriptionKeyId { get; set; }

        public byte[] Cover { get; set; }

        [StringLength(5)]
        public string CoverType { get; set; }

        public byte[] Trailer { get; set; }

        [StringLength(5)]
        public string TrailerType { get; set; }

        public int Deleted { get; set; }

        public bool NeedsInitialization { get; set; }

        [Required]
        [StringLength(18)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
                
        public virtual ICollection<MediaDeviceGroup> MediaDeviceGroup { get; set; }

        public virtual ICollection<MediaMachine> MediaMachine { get; set; }

        public virtual ICollection<MediaServiceLevel> MediaServiceLevel { get; set; }
    }
}
