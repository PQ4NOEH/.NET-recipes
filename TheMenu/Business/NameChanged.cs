
using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class NameChanged: IEvent
    {
        public Guid AggregateId { get; private set; }
        public readonly String NewName;
        public NameChanged(Guid aggregateId, NotNullOrWhiteSpaceString newName)
        {
            AggregateId = aggregateId;
            NewName = newName;
        }
    }
}
