
using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Davalor.MomProxy.Services
{
    public sealed class MessageForwarderFactory : IMessageForwarderFactory
    {
        readonly List<IMessageForwarder> _forwarders = new List<IMessageForwarder>();
        readonly IMomRepository _momRepository;
        readonly IServiceEvents _mediator;
        readonly IQuotaFactory _quotaFactory;

        public MessageForwarderFactory(
            NotNullable<IMomRepository> momRepository, 
            NotNullable<IServiceEvents> mediator, 
            NotNullable<IQuotaFactory> quotaFactory)
        { 
            _momRepository = momRepository.Value;
            _mediator = mediator.Value;
            _quotaFactory = quotaFactory.Value;
        }

        public IMessageForwarder CreateForwarder(NotNullOrWhiteSpaceString topic)
        {
            var fordwarder = _forwarders
                                .Where(f => f.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                                .FirstOrDefault();
            if(fordwarder == null)
            {
                fordwarder = new MessageForwarder(
                    topic,
                    new NotNullable<IMomRepository>(_momRepository),
                    new NotNullable<IServiceEvents>(_mediator),
                    _quotaFactory.CreateQuota(topic) 
                );
                _forwarders.Add(fordwarder);
            }
            return fordwarder;
        }
    }
}
