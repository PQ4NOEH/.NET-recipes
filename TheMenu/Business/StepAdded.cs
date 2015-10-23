
using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class StepAdded : Step, IEvent
    {
        public Guid AggregateId { get; private set; }
        public StepAdded(Guid aggregateId, uint order, NotNullOrWhiteSpaceString explanation,byte[] image)
            : base(order, explanation, image)
        {
            AggregateId = aggregateId;
        }
    }
}
