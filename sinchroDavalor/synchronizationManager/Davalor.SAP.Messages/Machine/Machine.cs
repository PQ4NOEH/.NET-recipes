using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Machine
{
    public partial class MachineAggregate : ISynchroAggregateRoot
    {
        public MachineAggregate()
        {
            MachinePrinter = new HashSet<MachinePrinter>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(18)]
        public string SerialNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceIdentifier { get; set; }

        [StringLength(50)]
        public string UserFriendlyName { get; set; }

        [Required]
        [StringLength(100)]
        public string DescriptionKeyId { get; set; }

        [StringLength(30)]
        public string Ip { get; set; }

        public int Port { get; set; }

        public int PortState { get; set; }

        public int StreamingPort { get; set; }

        public int SortOrder { get; set; }

        public int Deleted { get; set; }

        public Guid PartnerId { get; set; }

        public Guid MachineGroupId { get; set; }

        [Required]
        [StringLength(36)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual MachineGroup MachineGroup { get; set; }

        public virtual ICollection<MachinePrinter> MachinePrinter { get; set; }
    }
}
