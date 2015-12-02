using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Region
{
    public class RegisteredRegion : BaseEvent 
    {
        public RegisteredRegion()
        {
            Topic = "Region";
        }
    }
    public class ChangedRegion : BaseEvent 
    {
        public ChangedRegion()
        {
            Topic = "Region";
        }
    }
    public class UnregisteredRegion : BaseEvent 
    {
        public UnregisteredRegion()
        {
            Topic = "Region";
        }
    }
}
