using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Kafka;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Host.Configuration;
using Davalor.MomProxy.Repository;
using Davalor.MomProxy.Services;
using System;

namespace Davalor.MomProxy.ConsoleHost
{
    public class HostService : IDisposable
    {
        readonly IncommingMessageRepository _messagesRepository;
        readonly MessageForwardingService _messageForwardingService;
        readonly WebAPISelfHost _webApiHost;
        public HostService()
        {
            var kafkaProducerFactory = new KafkaProducerFactory(KafkaConfiguration.FromLocalFile("KafkaConfiguration.json"));
            var forwarderFactory = new MessageForwarderFactory(
                new MomRepository(kafkaProducerFactory),
                ServiceEvents.Instance.Value,
                new QuotaFactory(HostConfiguration.Instance.Value)
            );

            _messagesRepository = new IncommingMessageRepository();
            _messageForwardingService = new MessageForwardingService(new JsonSerializer(), ServiceEvents.Instance.Value, forwarderFactory);
            new MessageCleanerService(ServiceEvents.Instance.Value, _messagesRepository);
            _webApiHost = new WebAPISelfHost(HostConfiguration.Instance.Value.WebListenerPort);
        }
        public HostService(MessageForwardingService messageForwardingService, IncommingMessageRepository messageRepository)
        {
            _messagesRepository = messageRepository;
            _messageForwardingService = messageForwardingService;
            new MessageCleanerService(ServiceEvents.Instance.Value, _messagesRepository);
            _webApiHost = new WebAPISelfHost(HostConfiguration.Instance.Value.WebListenerPort);
        }

        public void Start()
        {
            _messageForwardingService
                .ProcessPendingMessages(_messagesRepository.GetPending())
                .StartListening();
            _webApiHost.StartListenning();
            MomProxyEventTracing.Log.Value.Service_started(HostConfiguration.Instance.Value.ApplicationName, HostConfiguration.Instance.Value.MachineName);
        }
        public void Stop()
        {
            _messageForwardingService.StopSendingMessages();
            _webApiHost.StopListeninning();
            MomProxyEventTracing.Log.Value.Service_stopped(HostConfiguration.Instance.Value.ApplicationName, HostConfiguration.Instance.Value.MachineName);
        }
        public void Pause()
        {
            _messageForwardingService.StopSendingMessages();
            _webApiHost.StopListeninning();
        }
        public void Continue()
        {
            _messageForwardingService.StartListening();
            _webApiHost.StartListenning();
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
