using Davalor.SynchronizationManager.Domain.Events;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Davalor.SynchronizationManager.Domain.UnitTests.Events
{
    public class ServiceEventsSpec
    {
        [Fact]
        public void Given_a_subscription_exists_when_emited_a_message_then_subscriber_gets_called()
        {
            var sut = new ServiceEvents();
            var callbackCalled = false;
            var incommingMessage = FakeMessage.GenerateIncomingEvent(FakeMessage.GenerateRandom());
            sut.IncommingEventSequence.Subscribe(i =>
                {
                    Assert.Same(i, incommingMessage);
                    callbackCalled = true;
                });
            sut.AddIncommingEvent(incommingMessage);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void Given_messages_are_emited_before_subscription_is_made_when_subscripted_then_receives_past_messages()
        {
            var sut = new ServiceEvents();
            List<IncommingEvent> events = new List<IncommingEvent>();
            var elements = (10).Times<IncommingEvent>(new Func<IncommingEvent>(() => FakeMessage.GenerateIncomingEvent(FakeMessage.GenerateRandom())));
            events.AddRange(elements);
            events.ForEach(e => sut.AddIncommingEvent(e));
            sut.IncommingEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Empty(events);
        }

        [Fact]
        public void Given_messages_are_emited_before_subscription_is_made_when_subscripted_then_receives_past_messages_not_older_then_ten()
        {
            var sut = new ServiceEvents();
            List<IncommingEvent> events = new List<IncommingEvent>();
            var elements = (11).Times<IncommingEvent>(new Func<IncommingEvent>(() => FakeMessage.GenerateIncomingEvent(FakeMessage.GenerateRandom())));
            events.AddRange(elements);
            events.ForEach(e => sut.AddIncommingEvent(e));
            sut.IncommingEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Equal(events.Count, 1);
        }

        [Fact]
        public void ReceivedMessageEventSequence()
        {
            var sut = new ServiceEvents();
            var callbackCalled = false;
            var message = new ReceivedMessage
            {
                EventID = Guid.NewGuid(),
                MessageHandler = StringExtension.RandomString(),
                MessageType = StringExtension.RandomString(),
                Topic = StringExtension.RandomString()
            };
            sut.ReceivedMessageEventSequence.Subscribe(i =>
            {
                Assert.Same(i, message);
                callbackCalled = true;
            });
            sut.AddReceivedMessageEvent(message);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void ReceivedMessageEventSequence_10()
        {
            var sut = new ServiceEvents();
            List<ReceivedMessage> events = new List<ReceivedMessage>();
            var elements = (10).Times<ReceivedMessage>(new Func<ReceivedMessage>(() =>
                {
                    return new ReceivedMessage
                    {
                        EventID = Guid.NewGuid(),
                        MessageHandler = StringExtension.RandomString(),
                        MessageType = StringExtension.RandomString(),
                        Topic = StringExtension.RandomString()
                    };
                }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddReceivedMessageEvent(e));
            sut.ReceivedMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Empty(events);
        }

        [Fact]
        public void ReceivedMessageEventSequence_11()
        {
            var sut = new ServiceEvents();
            List<ReceivedMessage> events = new List<ReceivedMessage>();
            var elements = (12).Times<ReceivedMessage>(new Func<ReceivedMessage>(() =>
            {
                return new ReceivedMessage
                {
                    EventID = Guid.NewGuid(),
                    MessageHandler = StringExtension.RandomString(),
                    MessageType = StringExtension.RandomString(),
                    Topic = StringExtension.RandomString()
                };
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddReceivedMessageEvent(e));
            sut.ReceivedMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Equal(events.Count, 2);
        }

        [Fact]
        public void ReceivedInvalidMessageEventSequence()
        {
            var sut = new ServiceEvents();
            var callbackCalled = false;
            var message = new ReceivedInvalidMessage
            {
                EventID = Guid.NewGuid(),
                MessageHandler = StringExtension.RandomString(),
                MessageType = StringExtension.RandomString(),
                Topic = StringExtension.RandomString()
            };
            sut.ReceivedInvalidMessageEventSequence.Subscribe(i =>
            {
                Assert.Same(i, message);
                callbackCalled = true;
            });
            sut.AddReceivedInvalidMessageEvent(message);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void ReceivedInvalidMessageEventSequence_10()
        {
            var sut = new ServiceEvents();
            List<ReceivedInvalidMessage> events = new List<ReceivedInvalidMessage>();
            var elements = (10).Times<ReceivedInvalidMessage>(new Func<ReceivedInvalidMessage>(() =>
            {
                return new ReceivedInvalidMessage
                {
                    EventID = Guid.NewGuid(),
                    MessageHandler = StringExtension.RandomString(),
                    MessageType = StringExtension.RandomString(),
                    Topic = StringExtension.RandomString()
                };
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddReceivedInvalidMessageEvent(e));
            sut.ReceivedInvalidMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Empty(events);
        }

        [Fact]
        public void ReceivedInvalidMessageEventSequence_11()
        {
            var sut = new ServiceEvents();
            List<ReceivedInvalidMessage> events = new List<ReceivedInvalidMessage>();
            var elements = (34).Times<ReceivedInvalidMessage>(new Func<ReceivedInvalidMessage>(() =>
            {
                return new ReceivedInvalidMessage
                {
                    EventID = Guid.NewGuid(),
                    MessageHandler = StringExtension.RandomString(),
                    MessageType = StringExtension.RandomString(),
                    Topic = StringExtension.RandomString()
                };
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddReceivedInvalidMessageEvent(e));
            sut.ReceivedInvalidMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Equal(events.Count, 24);
        }

        [Fact]
        public void ProcesedMessageEventSequence()
        {
            var sut = new ServiceEvents();
            var callbackCalled = false;
            var message = new ReceivedMessage
            {
                EventID = Guid.NewGuid(),
                MessageHandler = StringExtension.RandomString(),
                MessageType = StringExtension.RandomString(),
                Topic = StringExtension.RandomString()
            };
            sut.ProcesedMessageEventSequence.Subscribe(i =>
            {
                Assert.Same(i, message);
                callbackCalled = true;
            });
            sut.AddProcesedMessageEvent(message);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void ProcesedMessageEventSequence_10()
        {
            var sut = new ServiceEvents();
            List<ReceivedMessage> events = new List<ReceivedMessage>();
            var elements = (10).Times<ReceivedMessage>(new Func<ReceivedMessage>(() =>
            {
                return new ReceivedMessage
                {
                    EventID = Guid.NewGuid(),
                    MessageHandler = StringExtension.RandomString(),
                    MessageType = StringExtension.RandomString(),
                    Topic = StringExtension.RandomString()
                };
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddProcesedMessageEvent(e));
            sut.ProcesedMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Empty(events);
        }

        [Fact]
        public void ProcesedMessageEventSequence_11()
        {
            var sut = new ServiceEvents();
            List<ReceivedMessage> events = new List<ReceivedMessage>();
            var elements = (22).Times<ReceivedMessage>(new Func<ReceivedMessage>(() =>
            {
                return new ReceivedMessage
                {
                    EventID = Guid.NewGuid(),
                    MessageHandler = StringExtension.RandomString(),
                    MessageType = StringExtension.RandomString(),
                    Topic = StringExtension.RandomString()
                };
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddProcesedMessageEvent(e));
            sut.ProcesedMessageEventSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Equal(events.Count, 12);
        }

        [Fact]
        public void ProcesedMessageExceptionSequence()
        {
            var sut = new ServiceEvents();
            var callbackCalled = false;
            var message = new ProcesedMessageException(
                Guid.NewGuid(),
                StringExtension.RandomString(),
                StringExtension.RandomString(),
                StringExtension.RandomString(),
                new ArgumentException()
                );
            
            sut.ProcesedMessageExceptionSequence.Subscribe(i =>
            {
                Assert.Same(i, message);
                callbackCalled = true;
            });
            sut.AddProcesedMessageException(message);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void ProcesedMessageExceptionSequence_10()
        {
            var sut = new ServiceEvents();
            List<ProcesedMessageException> events = new List<ProcesedMessageException>();
            var elements = (10).Times<ProcesedMessageException>(new Func<ProcesedMessageException>(() =>
            {
                return new ProcesedMessageException(
                    Guid.NewGuid(),
                    StringExtension.RandomString(),
                    StringExtension.RandomString(),
                    StringExtension.RandomString(),
                    new ArgumentException()
                    );
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddProcesedMessageException(e));
            sut.ProcesedMessageExceptionSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Empty(events);
        }

        [Fact]
        public void ProcesedMessageExceptionSequence_11()
        {
            var sut = new ServiceEvents();
            List<ProcesedMessageException> events = new List<ProcesedMessageException>();
            var elements = (104).Times<ProcesedMessageException>(new Func<ProcesedMessageException>(() =>
            {
                return new ProcesedMessageException(
                    Guid.NewGuid(),
                    StringExtension.RandomString(),
                    StringExtension.RandomString(),
                    StringExtension.RandomString(),
                    new ArgumentException()
                    );
            }));
            events.AddRange(elements);
            events.ForEach(e => sut.AddProcesedMessageException(e));
            sut.ProcesedMessageExceptionSequence.Subscribe(i =>
            {
                Assert.True(events.Contains(i));
                events.Remove(i);
            });

            Assert.Equal(events.Count, 94);
        }
    }
}
