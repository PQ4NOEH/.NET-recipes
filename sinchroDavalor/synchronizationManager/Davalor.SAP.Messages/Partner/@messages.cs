using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Partner
{
    public class RegisteredPartner : BaseEvent
    {
        public RegisteredPartner()
        {
            Topic = "Partner";
        }
    }
    public class ChangedPartner : BaseEvent
    {
        public ChangedPartner()
        {
            Topic = "Partner";
        }
    }
}
