using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Device
{
    public partial class DeviceType
    {
        public DeviceType()
        {
            DeviceGroup = new HashSet<DeviceGroup>();
        }

        public Guid Id { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(18)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<DeviceGroup> DeviceGroup { get; set; }
    }
}
