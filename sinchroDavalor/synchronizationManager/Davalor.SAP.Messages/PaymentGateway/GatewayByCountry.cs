using System;

namespace Davalor.SAP.Messages.PaymentGateway
{
    public partial class GatewayByCountry
    {
        public Guid Id { get; set; }

        public Guid GatewayId { get; set; }

        public Guid CountryId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual GatewayAggregate Gateway { get; set; }
    }
}
