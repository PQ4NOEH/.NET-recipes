using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Service
{
    public class RegisteredService : BaseEvent 
    {
        public RegisteredService()
        {
            Topic = "Service";
        }
    }
    public class ChangedService : BaseEvent 
    {
        public ChangedService()
        {
            Topic = "Service";
        }
    }
    public class UnregisteredService : BaseEvent 
    {
        public UnregisteredService()
        {
            Topic = "Service";
        }
    }
}
