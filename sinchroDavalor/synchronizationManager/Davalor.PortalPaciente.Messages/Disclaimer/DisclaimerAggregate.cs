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

        public Guid Id { get; set; }         public Int32 Version { get; set; }

        [StringLength(20)]        public String Code { get; set; }

        [StringLength(100)]        public String LinkKeyId { get; set; }

        [StringLength(100)]        public String TextKeyId { get; set; }         public DateTimeOffset TimeStamp { get; set; } 
    }
}
