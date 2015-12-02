
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Media
{
    public class RegisteredMedia : BaseEvent
    {
        public RegisteredMedia()
        {
            Topic = "Media";
        }
    }
    public class ChangedMedia : BaseEvent
    {
        public ChangedMedia()
        {
            Topic = "Media";
        }
    }
    public class UnregisteredMedia : BaseEvent
    {
        public UnregisteredMedia()
        {
            Topic = "Media";
        }
    }
}
