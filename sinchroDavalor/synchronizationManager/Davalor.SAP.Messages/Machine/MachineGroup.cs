using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Machine
{
    public partial class MachineGroup
    {
        public MachineGroup()
        {
            Machine = new HashSet<MachineAggregate>();
        }

        public Guid Id { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public int Deleted { get; set; }

        [StringLength(50)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<MachineAggregate> Machine { get; set; }
    }
}
