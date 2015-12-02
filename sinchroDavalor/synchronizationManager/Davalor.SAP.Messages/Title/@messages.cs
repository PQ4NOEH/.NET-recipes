using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Title
{
    public class RegisteredTitle : BaseEvent 
    {
        public RegisteredTitle()
        {
            Topic = "Title";
        }
    }
    public class ChangedTitle : BaseEvent 
    {
        public ChangedTitle()
        {
            Topic = "Title";
        }
    }
    public class UnregisteredTitle : BaseEvent 
    {
        public UnregisteredTitle()
        {
            Topic = "Title";
        }
    }
}
