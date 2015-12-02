using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.PaymentGateway
{
    public partial class GatewayAggregate : ISynchroAggregateRoot
    {
        public GatewayAggregate()
        {
            GatewayByCountry = new HashSet<GatewayByCountry>();
        }

        public Guid Id { get; set; }

        public int GatewayType { get; set; }

        public int GatewayPlatformType { get; set; }

        [StringLength(1000)]
        public string GatewayTypeName { get; set; }

        [StringLength(1000)]
        public string GatewayDescription { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<GatewayByCountry> GatewayByCountry { get; set; }
    }
}
