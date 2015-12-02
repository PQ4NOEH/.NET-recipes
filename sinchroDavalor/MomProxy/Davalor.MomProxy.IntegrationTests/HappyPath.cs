using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Messaging.Kafka;
using Davalor.MomProxy.ConsoleHost;
using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Host.Configuration;
using Davalor.MomProxy.IntegrationTests.Helpers;
using Davalor.MomProxy.Repository;
using Davalor.MomProxy.Services;
using Davalor.Toolkit.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.MomProxy.IntegrationTests
{
    public class HappyPath
    {
        readonly BaseEvent _validMessage = new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = StringExtension.RandomString(),
                MessageType = StringExtension.RandomString(),
                Topic = StringExtension.RandomString()
            };
        readonly IStringSerializer _serializer = new JsonSerializer();
        [Fact]
        public async Task When_receives_a_message_persist_it()
        {
            IDisposable savedSubscription = null;
            try
            {
                using (HostService host = new HostService())
                {
                    host.Start();
                    bool savedIncomming = false;
                    savedSubscription = ServiceEvents.Instance.Value.SavedIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
                    {
                        Assert.Equal(_serializer.Serialize<BaseEvent>(_validMessage), s);
                        savedIncomming = true;
                    }));
                    await MessageRequestHelper.DoRequest(_validMessage, HostConfiguration.Instance.Value.WebListenerPort);
                    Assert.True(savedIncomming);
                }
            }
            finally
            {
                File.Delete("PendingMessages");
                if(savedSubscription != null) savedSubscription.Dispose();
            }
            
        }

        [Fact]
        public async Task When_receives_a_message_sends_it_to_the_mom()
        {
            try
            { 
                var momRepositoryFake = new MomRepositoryFake();
                var forwarderFactory = new MessageForwarderFactory(
                    momRepositoryFake,
                    ServiceEvents.Instance.Value,
                    new QuotaFactory(HostConfiguration.Instance.Value)
                );
                var messageForwardingService = new MessageForwardingService(new JsonSerializer(), ServiceEvents.Instance.Value, forwarderFactory);
                using (HostService host = new HostService(messageForwardingService, new IncommingMessageRepository()))
                {
                    host.Start();
                    await MessageRequestHelper.DoRequest(_validMessage, HostConfiguration.Instance.Value.WebListenerPort);
                    Thread.Sleep(1000);
                    Assert.True(momRepositoryFake.ReceivedSendMessages.ContainsKey(_validMessage.Topic));
                    Assert.Equal(momRepositoryFake.ReceivedSendMessages[_validMessage.Topic].Count, 1);
                    Assert.Equal(momRepositoryFake.ReceivedSendMessages[_validMessage.Topic].First(), _serializer.Serialize<BaseEvent>(_validMessage));
                }
            }
            finally
            {
                File.Delete("PendingMessages");
            }
        }
        [Fact]
        public async Task When_receives_a_message_sends_it_to_the_mom_and_it_is_avalaible_on_the_mom()
        {
            IDisposable listenerSubscription = null; 
            try
            {
                using (HostService host = new HostService())
                {
                    host.Start();
                    bool messageListened = false;
                    var topic = StringExtension.RandomString();
                    var serializer = new JsonSerializer();
                    _validMessage.Topic = topic;
                    var serializedMessage = _serializer.Serialize<BaseEvent>(_validMessage);
                    await MessageRequestHelper.DoRequest(_validMessage, HostConfiguration.Instance.Value.WebListenerPort);
                    var kafkaListener = new KafkaListener<BaseEvent>(
                        new BinaryJsonSerializer(),
                        new KafkaOffsetLocalRepository("offsets", topic),
                        new KafkaConsumerFactory(KafkaConfiguration.FromLocalFile()),
                        topic
                    );
                    listenerSubscription = kafkaListener.ListenedMessages.Subscribe(new Action<BaseEvent>(s =>
                    {
                        messageListened = s.EventID == _validMessage.EventID;
                    }));
                    kafkaListener.StartListening();
                    while (!messageListened)  Thread.Sleep(100);
                    kafkaListener.StopListening();
                }
            }
            finally
            {
                File.Delete("PendingMessages");
                if (listenerSubscription != null) listenerSubscription.Dispose();
            }
        }
        [Fact]
        public async Task When_a_message_has_been_send_to_the_mom_it_is_removed_from_the_local_storage()
        {
            IDisposable savedSubscription = null;
            IDisposable sentSubscription = null;
            var serializedMessage = _serializer.Serialize<BaseEvent>(_validMessage);
            try 
            { 
                using (HostService host = new HostService())
                {
                    host.Start();
                    bool sentIncommingMessage = false;
                    savedSubscription = ServiceEvents.Instance.Value.SavedIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
                    {
                        string[] persistedMessages = File.ReadAllLines("PendingMessages");
                        Assert.Equal(persistedMessages.Count(), 1);
                        Assert.Equal(persistedMessages.First(), serializedMessage);
                    }));
                    sentSubscription = ServiceEvents.Instance.Value.SentIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
                    {
                        string[] persistedMessages = File.ReadAllLines("PendingMessages");
                        Assert.Equal(persistedMessages.Count(), 0);
                        sentIncommingMessage = true;
                    }));
                    await MessageRequestHelper.DoRequest(_validMessage, HostConfiguration.Instance.Value.WebListenerPort);
                    Thread.Sleep(1000);
                    Assert.True(sentIncommingMessage);
                }
            }
            finally
            {
                File.Delete("PendingMessages");
                if (savedSubscription != null) savedSubscription.Dispose();
                if (sentSubscription != null) sentSubscription.Dispose();
            }
        }

    }
}
