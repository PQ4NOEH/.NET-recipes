
using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Services;
using Davalor.MomProxy.Services;
using Davalor.Toolkit.Extensions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.UnitTest.Forwarder
{
    public class MessageForwardingServiceSpec
    {
        [Fact]
        public void It_can_process_pending_messages()
        {
            var serializer = new NotNullable<IStringSerializer>(new JsonSerializer());
            var forwarderFactory = new MessageForwarderFactoryFake();
            var serviceEvents = new ServiceEvents();
            var sut = new MessageForwardingService(serializer, serviceEvents, forwarderFactory);

            var messages = GenerateMessages(8, "oneTopic");
            sut.ProcessPendingMessages(messages);

            Assert.NotNull(forwarderFactory.Forwarders["oneTopic"]);
            forwarderFactory.Forwarders["oneTopic"].Received(messages.Value.Count()).AddMessage(Arg.Any<string>());
        }
         [Fact]
        public void It_routes_each_message_by_topic()
        {
            var serializer = new NotNullable<IStringSerializer>(new JsonSerializer());
            var forwarderFactory = new MessageForwarderFactoryFake();
            var serviceEvents = new ServiceEvents();
            var sut = new MessageForwardingService(serializer, serviceEvents, forwarderFactory);

            var messages = GenerateMessages(8);
            sut.ProcessPendingMessages(messages);
            var groupedTopics = messages.Value
                                            .GroupBy(m => serializer.Value.Deserialize<BaseEvent>(m).Topic)
                                            .Select(g => new {
                                                Name = g.Key,
                                                Count = g.Count()
                                            });
            Assert.NotEmpty(groupedTopics);
            groupedTopics.ToList().ForEach(topicCount =>
            {
                Assert.NotNull(forwarderFactory.Forwarders[topicCount.Name]);
                forwarderFactory.Forwarders[topicCount.Name].Received(topicCount.Count).AddMessage(Arg.Any<string>());
            });
        }

        [Fact]
        public void It_wont_process_any_incomming_message_until_we_call_StartListening()
        {
            var serializer = new NotNullable<IStringSerializer>(new JsonSerializer());
            var forwarderFactory = new MessageForwarderFactoryFake();
            var serviceEvents = new ServiceEvents();
            var sut = new MessageForwardingService(serializer, serviceEvents, forwarderFactory);
            serviceEvents.SavedIncommingMessage(GenerateMessages(1, "ATopic").Value.ElementAt(0));
            Assert.False(forwarderFactory.Forwarders.ContainsKey("ATopic"));
        }

        [Fact]
        public void when_called_StartListening_it_will_process_every_incommingEvent()
        {
            var serializer = new NotNullable<IStringSerializer>(new JsonSerializer());
            var forwarderFactory = new MessageForwarderFactoryFake();
            var serviceEvents = new ServiceEvents();
            var sut = new MessageForwardingService(serializer, serviceEvents, forwarderFactory);
            sut.StartListening();
            serviceEvents.SavedIncommingMessage(GenerateMessages(1, "ATopic").Value.ElementAt(0));
            Assert.NotNull(forwarderFactory.Forwarders["ATopic"]);
            forwarderFactory.Forwarders["ATopic"].Received(1).AddMessage(Arg.Any<string>());
        }

        [Fact]
        public void StopSendingMessages_stops_every_awakened_forwarder()
        {
            var serializer = new NotNullable<IStringSerializer>(new JsonSerializer());
            var forwarderFactory = new MessageForwarderFactoryFake();

            var serviceEvents = new ServiceEvents();
            var sut = new MessageForwardingService(serializer, serviceEvents, forwarderFactory);

            var messages = GenerateMessages(8);
            sut.ProcessPendingMessages(messages);
            sut.StopSendingMessages();

            Assert.True(forwarderFactory.Forwarders.Any());
            forwarderFactory.Forwarders.ToList().ForEach(item => item.Value.Received().StopForwarding());
        }

       

        NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> GenerateMessages(int numberOfMessages, string topic = null)
        {
            List<NotNullOrWhiteSpaceString> messages = new List<NotNullOrWhiteSpaceString>();

            numberOfMessages.Times(new Action(() =>
                {
                    messages.Add(new JsonSerializer().Serialize<BaseEvent>(
                        new BaseEvent
                        {
                            EventID = Guid.NewGuid(),
                            Topic = topic ?? StringExtension.RandomString()
                        })
                    );
                }));
            return new NotNullable<IEnumerable<NotNullOrWhiteSpaceString>>(messages);
        }


        class MessageForwarderFactoryFake : IMessageForwarderFactory
        {
            public Dictionary<string, IMessageForwarder> Forwarders = new Dictionary<string, IMessageForwarder>();
            public IMessageForwarder CreateForwarder(NotNullOrWhiteSpaceString topic)
            {
                if (!Forwarders.ContainsKey(topic)) Forwarders.Add(topic, Substitute.For<IMessageForwarder>());
                return Forwarders[topic];
            }
        }
    }
}
