using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Device
{
    public partial class DeviceGroup
    {
        public DeviceGroup()
        {
            Device = new HashSet<DeviceAggregate>();
        }

        public Guid Id { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public Guid DeviceTypeId { get; set; }

        [Required]
        [StringLength(26)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<DeviceAggregate> Device { get; set; }

        public virtual DeviceType DeviceType { get; set; }
    }
}
