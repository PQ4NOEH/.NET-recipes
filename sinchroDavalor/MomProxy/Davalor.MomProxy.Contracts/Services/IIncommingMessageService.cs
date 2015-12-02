
using Davalor.Base.Library.Guards;
using System;
namespace Davalor.MomProxy.Domain.Services
{
    public interface IIncommingMessageService
    {
        void NewMessage(NotNullOrWhiteSpaceString incommingMessage);
    }
}
