using System;

namespace Davalor.SAP.Messages.Machine
{
    public partial class MachinePrinter
    {
        public Guid Id { get; set; }

        public Guid MachineId { get; set; }

        public Guid PrinterId { get; set; }

        public int Function { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual MachineAggregate Machine { get; set; }
    }
}
