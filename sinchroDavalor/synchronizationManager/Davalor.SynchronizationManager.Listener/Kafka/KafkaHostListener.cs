using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.Listeners;
using System.Collections.Generic;
using System;
using Davalor.Base.Messaging.Kafka.Contracts;
using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.Listener.Kafka
{
    /// <summary>
    /// Manager for Kafka listeners messages
    /// </summary>
    public class KafkaHostListener : IKafkaHostListener
    {
        readonly IServiceEvents _serviceEvents;
        readonly IKafkaListenerFactory _listenerFactory;
        readonly IBinarySerializer _serializer;
        readonly Dictionary<string, IKafkaListener> _listeners = new Dictionary<string, IKafkaListener>();
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceEvents">ServiceEvents</param>
        /// <param name="listenerFactory">KafkaListener factory</param>
        /// <param name="serializer">Binary serializer</param>
        public KafkaHostListener(
            IServiceEvents serviceEvents,
            IKafkaListenerFactory listenerFactory,
            IBinarySerializer serializer)
        {
            _serviceEvents = serviceEvents;
            _listenerFactory = listenerFactory;
            _serializer = serializer;
        }
        /// <summary>
        /// Allows to hook for a given topic
        /// </summary>
        /// <param name="topicName">The topic which want to be listened to</param>
        public void ListenToTopic(NotNullOrWhiteSpaceString topicName)
        {
            if(!_listeners.ContainsKey(topicName))
            {
                var newListener = _listenerFactory.CreateInstance(topicName);
                newListener.StartListening();
                newListener.ListenedMessages.Subscribe((message) => ManageListenedMessages(message, topicName));
                _listeners.Add(topicName, newListener);
            }
        }
        /// <summary>
        /// Deserializes a message and emit it to serviceEvents
        /// </summary>
        /// <param name="message">The message listened</param>
        /// <param name="topic">The topic in which the message was listened</param>
        void ManageListenedMessages(byte[] message, NotNullOrWhiteSpaceString topic)
        {
            BaseEvent @event = null;
            try
            {
                @event = _serializer.Deserialize<BaseEvent>(message);
                if(@event.IsValid())
                {
                     var incommingEvent = new IncommingEvent
                    {
                        @event = @event,
                        RawData = message
                    };
                    _serviceEvents.AddIncommingEvent(incommingEvent);
                }
                else
                {
                    var receivedInvalidMessage = new ReceivedInvalidMessage
                    {
                        EventID = @event.EventID,
                        MessageType = @event.MessageType,
                        Topic = @event.Topic,
                        InvalidReason = @event.InvalidReason.Value
                    };
                    _serviceEvents.AddReceivedInvalidMessageEvent(receivedInvalidMessage);
                }
            }
            catch
            {
                var receivedInvalidMessage = new ReceivedInvalidMessage
                {
                  InvalidReason = EInvalidMessageReason.Undeserializable,
                  Topic = topic
                };
                _serviceEvents.AddReceivedInvalidMessageEvent(receivedInvalidMessage);
            }
        }
    }
}
