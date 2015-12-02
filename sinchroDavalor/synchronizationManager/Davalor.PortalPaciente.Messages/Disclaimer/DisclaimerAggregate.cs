using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.Disclaimer
{
    public partial class DisclaimerAggregate : ISynchroAggregateRoot
    {
        public DisclaimerAggregate()
        {
        }

        public Guid Id { get; set; } 

        [StringLength(20)]

        [StringLength(100)]

        [StringLength(100)]
    }
}