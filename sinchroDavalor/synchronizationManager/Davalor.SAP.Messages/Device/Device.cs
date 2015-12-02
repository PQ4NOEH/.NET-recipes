
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Device
{
    public partial class DeviceAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }

        [StringLength(100)]
        public string DescriptionKeyId { get; set; }

        [Required]
        [StringLength(18)]
        public string SerialNumber { get; set; }

        public int Deleted { get; set; }

        public Guid DeviceGroupId { get; set; }

        public Guid MachineId { get; set; }

        [Required]
        [StringLength(36)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual DeviceGroup DeviceGroup { get; set; }
    }
}
