using System;

namespace Davalor.SAP.Messages.Media
{
    public partial class MediaDeviceGroup
    {
        public Guid Id { get; set; }

        public Guid DeviceGroupId { get; set; }

        public int Deleted { get; set; }

        public Guid MediaId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual MediaAggregate Media { get; set; }
    }
}
