using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Location
{
    public class RegisteredLocation : BaseEvent
    {
        public RegisteredLocation()
        {
            Topic = "Location";
        }
    }
    public class ChangedLocation : BaseEvent
    {
        public ChangedLocation()
        {
            Topic = "Location";
        }
    }
    public class UnregisteredLocation : BaseEvent
    {
        public UnregisteredLocation()
        {
            Topic = "Location";
        }
    }
}
