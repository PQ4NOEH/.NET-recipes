using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheMenu.Core
{
    public interface IEventStore
    {
        Task AppendEvent(string bucketId, IEvent @event);
        Task AppendEvents(string bucketId, IEnumerable<IEvent> @event);
        Task<IEnumerable<IEvent>> GetEventsFromBucket(string bucketId);
    }
}
