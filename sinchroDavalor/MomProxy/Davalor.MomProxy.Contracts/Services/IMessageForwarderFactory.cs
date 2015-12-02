using Davalor.Base.Library.Guards;
using System;
namespace Davalor.MomProxy.Domain.Services
{
    public interface IMessageForwarderFactory
    {
        IMessageForwarder CreateForwarder(NotNullOrWhiteSpaceString topic);
    }
}
