
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Listener.Kafka;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reactive.Subjects;
using Davalor.Base.Library.Serialization;
using Davalor.Toolkit.Extensions;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Messaging.Kafka.Contracts;
using Davalor.Base.Library.Guards;
using Davalor.Base.Contract.Library;

namespace Davalor.SynchronizationManager.Listener.UnitTests.Kafka
{
    public class KafkaHostListenerSpec
    {
        [Fact]
        public void Given_a_given_topic_when_a_message_is_published_on_to_that_topic_then_it_emits_IncommingEvent_event()
        {
            IServiceEvents serviceEvents = new ServiceEvents();
            var calledCallback = false;
            var listenerFactory = new ListenerFactoryFake();
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            sut.ListenToTopic(StringExtension.RandomString());
            var message = GenerateRandomBaseEvent();
            listenerFactory.Instance.MessagesSequence.OnNext(serializer.Serialize<BaseEvent>(message));
            serviceEvents.IncommingEventSequence.Subscribe(m =>
                {
                    Assert.Equal(message.EventID, m.@event.EventID);
                    Assert.Equal(message.MessageType, m.@event.MessageType);
                    Assert.Equal(message.Topic, m.@event.Topic);
                    calledCallback = true;
                });

            Assert.True(calledCallback);
        }

        [Fact]
        public void Given_the_same_topic_is_provided_n_times_it_only_does_one_subscription()
        {
            IServiceEvents serviceEvents = Substitute.For<IServiceEvents>();
            var listenerFactory = Substitute.For<IKafkaListenerFactory>();
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            var topic = StringExtension.RandomString();
            new Random().Next(2, 30).Times(new Action(() => sut.ListenToTopic(topic)));
            listenerFactory.Received(1).CreateInstance(Arg.Any<NotNullOrWhiteSpaceString>());
        }

        [Fact]
        public void Given_an_undesrializable_event_is_listened_when_managed_then_emits_receivedInvalidMessage_event()
        {
            IServiceEvents serviceEvents = new ServiceEvents();
            var listenerFactory = new ListenerFactoryFake();
            var calledCallback = false;
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            var topic = StringExtension.RandomString();
            sut.ListenToTopic(topic);
            var message = Encoding.UTF8.GetBytes(topic);
            listenerFactory.Instance.MessagesSequence.OnNext(message);
            serviceEvents.ReceivedInvalidMessageEventSequence.Subscribe(m =>
                {
                    Assert.Equal(m.InvalidReason, EInvalidMessageReason.Undeserializable);
                    Assert.Equal(m.Topic, topic);
                    calledCallback = true;

                });
            Assert.True(calledCallback);
        }

        [Fact]
        public void Given_an_event_without_EventId_when_managed_then_emits_receivedInvalidMessage_event()
        {
            IServiceEvents serviceEvents = new ServiceEvents();
            var listenerFactory = new ListenerFactoryFake();
            var calledCallback = false;
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            var topic = StringExtension.RandomString();
            sut.ListenToTopic(topic);
            var message = GenerateRandomBaseEvent();
            message.EventID = Guid.Empty;
            listenerFactory.Instance.MessagesSequence.OnNext(serializer.Serialize<BaseEvent>(message));
            serviceEvents.ReceivedInvalidMessageEventSequence.Subscribe(m =>
            {
                Assert.Equal(m.InvalidReason, EInvalidMessageReason.EmptyId);
                Assert.Equal(m.Topic, message.Topic);
                Assert.Equal(m.MessageType, message.MessageType);
                calledCallback = true;

            });
            Assert.True(calledCallback);
        }

        [Fact]
        public void Given_an_event_without_topic_when_managed_then_emits_receivedInvalidMessage_event()
        {
            IServiceEvents serviceEvents = new ServiceEvents();
            var listenerFactory = new ListenerFactoryFake();
            var calledCallback = false;
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            var topic = StringExtension.RandomString();
            sut.ListenToTopic(topic);
            var message = GenerateRandomBaseEvent();
            message.Topic = null;
            listenerFactory.Instance.MessagesSequence.OnNext(serializer.Serialize<BaseEvent>(message));
            serviceEvents.ReceivedInvalidMessageEventSequence.Subscribe(m =>
            {
                Assert.Equal(m.InvalidReason, EInvalidMessageReason.EmptyTopic);
                Assert.Equal(m.EventID, message.EventID);
                Assert.Equal(m.MessageType, message.MessageType);
                calledCallback = true;

            });
            Assert.True(calledCallback);
        }

        [Fact]
        public void Given_an_event_without_messageType_when_managed_then_emits_receivedInvalidMessage_event()
        {
            IServiceEvents serviceEvents = new ServiceEvents();
            var listenerFactory = new ListenerFactoryFake();
            var calledCallback = false;
            var serializer = new BinaryJsonSerializer();
            var sut = new KafkaHostListener(serviceEvents, listenerFactory, serializer);
            var topic = StringExtension.RandomString();
            sut.ListenToTopic(topic);
            var message = GenerateRandomBaseEvent();
            message.MessageType = null;
            listenerFactory.Instance.MessagesSequence.OnNext(serializer.Serialize<BaseEvent>(message));
            serviceEvents.ReceivedInvalidMessageEventSequence.Subscribe(m =>
            {
                Assert.Equal(m.InvalidReason, EInvalidMessageReason.EmptyMessageType);
                Assert.Equal(m.EventID, message.EventID);
                Assert.Equal(m.Topic, message.Topic);
                calledCallback = true;

            });
            Assert.True(calledCallback);
        }

        BaseEvent GenerateRandomBaseEvent()
        {
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageType = StringExtension.RandomString(),
                Topic = StringExtension.RandomString()
            };
        }
        class ListenerFactoryFake : IKafkaListenerFactory
        {
            public ListenerFake Instance = new ListenerFake();
            public IKafkaListener CreateInstance(NotNullOrWhiteSpaceString topicName)
            {
                return Instance;
            }

            public IKafkaListener<T> CreateInstance<T>(NotNullable<IBinarySerializer> serializer, NotNullOrWhiteSpaceString topicName)
            {
                throw new NotImplementedException();
            }
        }

        class ListenerFake : IKafkaListener
        {
            public readonly Subject<byte[]> MessagesSequence = new Subject<byte[]>();

            public int StartListeningCalledCount {get; private set;}
            public int StopListeningCalledCount { get; private set; } 

            public IObservable<byte[]> ListenedMessages
            {
                get { return MessagesSequence; }
            }

            public void StartListening()
            {
                StartListeningCalledCount++;
            }

            public void StopListening()
            {
                StopListeningCalledCount++;
            }
        }
    }
}
