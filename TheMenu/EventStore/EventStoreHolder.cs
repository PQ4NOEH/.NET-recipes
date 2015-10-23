using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheMenu.Core;

namespace EventStore
{
    public class EventStoreHolder : IEventStore
    {
        readonly IEventStoreConnection _connection;
        readonly IBinarySerializer _serializer;
        
        EventStoreHolder(IHostConfiguration configuration, IBinarySerializer serializer)
        {
            var ipEndpoint = new IPEndPoint(configuration.EventStoreIp, configuration.EventStorePort);

            _connection = EventStoreConnection.Create(ipEndpoint);
            _serializer = serializer;
        }

        public async Task AppendEvent(string bucketId, IEvent @event)
        {
            await AppendEvent(bucketId, @event);
        }

        public async Task AppendEvents(string bucketId, IEnumerable<IEvent> @event) 
        {
            await _connection.ConnectAsync();

            var eventsData = @event.Select(e => {
                return new EventData(
                    Guid.NewGuid(),
                    e.GetType().FullName,
                    false,
                    _serializer.Serialize(e),
                    null
                );
            });

            await _connection.AppendToStreamAsync(bucketId, ExpectedVersion.Any, eventsData);
        }

        public async Task<IEnumerable<IEvent>> GetEventsFromBucket(string bucketId)
        {
            await _connection.ConnectAsync();
            var eventsSlice = await _connection.ReadStreamEventsForwardAsync(bucketId, 0, int.MaxValue, false);

            return eventsSlice.Events.Select(e =>
                {   
                    var eventType = Type.GetType(e.OriginalEvent.EventType, true);
                    return _serializer.Deserialize(eventType, e.OriginalEvent.Data) as IEvent;
                });
        }
    }
}
