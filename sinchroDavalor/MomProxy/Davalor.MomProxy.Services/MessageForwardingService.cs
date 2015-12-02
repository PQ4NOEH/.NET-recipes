using Davalor.MomProxy.Domain.Services;
using Davalor.MomProxy.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.MomProxy.Services
{
    public class MessageForwardingService : IMessageForwardingService 
    {
        readonly List<IMessageForwarder> _forwarders;
        readonly IMessageForwarderFactory _messageForwarderFactory;
        readonly IStringSerializer _messageSerializer;
        readonly IServiceEvents _hostMediator;
        public MessageForwardingService(
            NotNullable<IStringSerializer> messageSerializer, 
            NotNullable<IServiceEvents> hostMediator,
            NotNullable<IMessageForwarderFactory> messageForwarderFactory)
        {
            _messageSerializer = messageSerializer.Value;
            _messageForwarderFactory = messageForwarderFactory.Value;
            _forwarders = new List<IMessageForwarder>();
            _hostMediator = hostMediator.Value;
        }

        public IMessageForwardingService StopSendingMessages()
        {
            _forwarders.ForEach(f => f.StopForwarding());
            return this;
        }

        public IMessageForwardingService ProcessPendingMessages(NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> pendingMessages)
        {
            pendingMessages
                .Value
                .ToList()
                .ForEach(m => AddMessage(m));

            return this;
        }
        public IMessageForwardingService StartListening()
        {
            _hostMediator.SavedIncommingMessageSequence.Subscribe(i => AddMessage(i));
            return this;
        }
        void AddMessage(string serializedMessage)
        {
            var message = _messageSerializer.Deserialize<BaseEvent>(serializedMessage);
            var forwarder = _forwarders
                                .Where(f => f.Topic.Equals(message.Topic, StringComparison.OrdinalIgnoreCase))
                                .FirstOrDefault();

            if (forwarder == null)
            {
                forwarder = _messageForwarderFactory.CreateForwarder(message.Topic);
                forwarder.StartForwarding();
                _forwarders.Add(forwarder);
            }
            forwarder.AddMessage(serializedMessage);
        }
    }
}
