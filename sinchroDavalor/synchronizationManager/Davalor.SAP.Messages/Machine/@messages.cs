using Davalor.Base.Messaging.Contracts;


namespace Davalor.SAP.Messages.Machine
{
    public class RegisteredMachine : BaseEvent
    {
        public RegisteredMachine()
        {
            Topic = "Machine";
        }
    }
    public class ChangedMachine : BaseEvent
    {
        public ChangedMachine()
        {
            Topic = "Machine";
        }
    }
    public class UnregisteredMachine : BaseEvent
    {
        public UnregisteredMachine()
        {
            Topic = "Machine";
        }
    }
}
