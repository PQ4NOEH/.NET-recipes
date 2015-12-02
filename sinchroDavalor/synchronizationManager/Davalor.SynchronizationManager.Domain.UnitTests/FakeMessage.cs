using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.Toolkit.Extensions;
using System;

namespace Davalor.SynchronizationManager.Domain.UnitTests
{
    public class FakeMessage
    {
        public Guid EventID { get; set; }
        public string Topic { get; set; }
        public string AString { get; set; }
        public Guid AId { get; set; }
        public string MessageType { get; set; }

        public static IncommingEvent GenerateIncomingEvent(FakeMessage message)
        {
            var serializer = new BinaryJsonSerializer();
            var messageSerialized = serializer.Serialize<FakeMessage>(message);
            return new IncommingEvent()
            {
                RawData = messageSerialized,
                @event = new BinaryJsonSerializer().Deserialize<BaseEvent>(messageSerialized)
            };
        }
        public static FakeMessage GenerateRandom()
        {
            return new FakeMessage
            {
                MessageType = typeof(FakeMessage).Name,
                AString = StringExtension.RandomString(),
                AId = Guid.NewGuid(),
                EventID = Guid.NewGuid(),
                Topic = StringExtension.RandomString()
            };
        }
    }
}
