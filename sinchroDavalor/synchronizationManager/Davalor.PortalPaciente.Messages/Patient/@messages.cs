
using Davalor.Base.Messaging.Contracts;
namespace Davalor.PortalPaciente.Messages.Patient
{
    public class RegisteredPatient : BaseEvent
    {
        public RegisteredPatient()
        {
            Topic = "Patient";
        }
    }
    public class ChangedPatient : BaseEvent
    {
        public ChangedPatient()
        {
            Topic = "Patient";
        }
    }
    public class UnregisteredPatient : BaseEvent
    {
        public UnregisteredPatient()
        {
            Topic = "Patient";
        }
    }
}
