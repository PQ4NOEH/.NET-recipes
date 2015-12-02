

using Davalor.Base.Messaging.Contracts;
namespace Davalor.PortalPaciente.Messages.Payable
{
    public class RegisteredPayable : BaseEvent 
    {
        public RegisteredPayable()
        {
            Topic = "Payable";
        }
    }
    public class ChangedPayable : BaseEvent  
    {
        public ChangedPayable()
        {
            Topic = "Payable";
        }
    }
    public class UnregisteredPayable : BaseEvent   
    {
        public UnregisteredPayable()
        {
            Topic = "Payable";
        }
    }
}
