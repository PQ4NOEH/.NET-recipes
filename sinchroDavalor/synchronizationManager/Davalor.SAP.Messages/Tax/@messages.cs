using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Tax
{
    public class RegisteredTax : BaseEvent 
    {
        public RegisteredTax()
        {
            Topic = "Tax";
        }
    }
    public class ChangedTax : BaseEvent 
    {
        public ChangedTax()
        {
            Topic = "Tax";
        }
    }
    public class UnregisteredTax : BaseEvent 
    {
        public UnregisteredTax()
        {
            Topic = "Tax";
        }
    }
}
