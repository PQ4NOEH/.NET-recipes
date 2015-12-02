
using Davalor.Base.Library.Guards;
using System.Collections.Generic;
namespace Davalor.MomProxy.Domain.Services
{
    public interface IMessageForwardingService
    {
        IMessageForwardingService StopSendingMessages();
        IMessageForwardingService ProcessPendingMessages(NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> pendingMessages);
        IMessageForwardingService StartListening();
    }
}
