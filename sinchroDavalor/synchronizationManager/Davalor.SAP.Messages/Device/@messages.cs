

using Davalor.Base.Messaging.Contracts;
namespace Davalor.SAP.Messages.Device
{
    public class RegisteredDevice : BaseEvent
    {
        public RegisteredDevice()
        {
            Topic = "Device";
        }
    }
    public class ChangedDevice : BaseEvent
    {
        public ChangedDevice()
        {
            Topic = "Device";
        }
    }
    public class UnregisteredDevice : BaseEvent
    {
        public UnregisteredDevice()
        {
            Topic = "Device";
        }
    }
}
