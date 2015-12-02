using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Printer
{
    public class RegisteredPrinter : BaseEvent
    {
        public RegisteredPrinter()
        {
            Topic = "Printer";
        }
    }
    public class ChangedPrinter : BaseEvent
    {
        public ChangedPrinter()
        {
            Topic = "Printer";
        }
    }
    public class UnregisteredPrinter : BaseEvent
    {
        public UnregisteredPrinter()
        {
            Topic = "Printer";
        }
    }
}
