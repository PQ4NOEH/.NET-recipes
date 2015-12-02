
using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain.Services;
using System;
namespace Davalor.MomProxy.Services
{
    public class IncommingMessageService : IIncommingMessageService
    {
        readonly IIncommingMessageRepository _repository;
        readonly IServiceEvents _hostMediator;

        public IncommingMessageService(IIncommingMessageRepository repository, IServiceEvents hostMediator)
        {
            _repository = repository;
            _hostMediator = hostMediator;
        }


        public void NewMessage(NotNullOrWhiteSpaceString incommingMessage)
        {
            _repository.Save(incommingMessage);
            _hostMediator.SavedIncommingMessage(incommingMessage);
            
        }
    }
}
