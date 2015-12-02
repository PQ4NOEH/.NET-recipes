
using Davalor.Base.Messaging.Contracts;
namespace Davalor.PortalPaciente.Messages.Answer
{
    public class RegisteredAnswer : BaseEvent
    {
        public RegisteredAnswer()
        {
            Topic = "Answer";
        }
    }
    public class ChangedAnswer : BaseEvent
    {
        public ChangedAnswer()
        {
            Topic = "Answer";
        }
    }
    public class UnregisteredAnswer : BaseEvent
    {
        public UnregisteredAnswer()
        {
            Topic = "Answer";
        }
    }
}
