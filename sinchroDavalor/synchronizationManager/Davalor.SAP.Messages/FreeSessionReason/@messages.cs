using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.FreeSessionReason
{
    public class RegisteredFreeSessionReason : BaseEvent
    {
        public RegisteredFreeSessionReason()
        {
            Topic = "FreeSessionReason";
        }
    }
    public class ChangedFreeSessionReason : BaseEvent
    {
        public ChangedFreeSessionReason()
        {
            Topic = "FreeSessionReason";
        }
    }
    public class UnregisteredFreeSessionReason : BaseEvent
    {
        public UnregisteredFreeSessionReason()
        {
            Topic = "FreeSessionReason";
        }
    }
}
