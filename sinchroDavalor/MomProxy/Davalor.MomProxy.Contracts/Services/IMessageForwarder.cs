using System;
namespace Davalor.MomProxy.Domain.Services
{
    public interface IMessageForwarder
    {
        void AddMessage(string message);
        void StartForwarding();
        void StopForwarding();
        string Topic { get; }
    }
}
