using System;

namespace Davalor.SAP.Messages.Media
{
    public partial class MediaMachine
    {
        public Guid Id { get; set; }

        public Guid MachineId { get; set; }

        public Guid MediaId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual MediaAggregate Media { get; set; }
    }
}
