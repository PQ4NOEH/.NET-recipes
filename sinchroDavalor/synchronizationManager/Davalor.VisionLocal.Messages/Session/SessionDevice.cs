using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.VisionLocal.Messages.Session
{
    public partial class SessionDevice
    {
        public Guid Id { get; set; }

        [StringLength(36)]
        public string SapCode { get; set; }

        [StringLength(18)]
        public string SerialNumber { get; set; }

        [StringLength(26)]
        public string DeviceGroup { get; set; }

        public Guid? DeviceId { get; set; }

        public Guid SessionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual SessionAggregate Session { get; set; }
    }
}
