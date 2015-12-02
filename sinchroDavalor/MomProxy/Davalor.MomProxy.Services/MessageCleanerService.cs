using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Services;
using System;

namespace Davalor.MomProxy.Services
{
    public class MessageCleanerService : IMessageCleanerService
    {
        readonly IIncommingMessageRepository _repository;
        readonly IServiceEvents _serviceEvents;
        public MessageCleanerService(NotNullable<IServiceEvents> serviceEvents, NotNullable<IIncommingMessageRepository> repository)
        {
            serviceEvents.Value.SentIncommingMessageSequence.Subscribe(RemoveMessage);
            _repository = repository.Value;
            _serviceEvents = serviceEvents.Value;
        }

        void RemoveMessage(NotNullOrWhiteSpaceString mesage)
        {
            _repository.Delete(mesage);
            _serviceEvents.DeletedIncommingMessage(mesage);

        }
    }
}
