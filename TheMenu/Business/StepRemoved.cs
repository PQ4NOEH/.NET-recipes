using System;
using TheMenu.Core;

namespace Business
{
    public class StepRemoved :IEvent
    {
        public Guid AggregateId { get; private set; }
        public readonly uint Order;
        public StepRemoved(Guid aggregateId, uint order)
        {
            AggregateId = aggregateId;
            Order = order;
        }
    }
}
