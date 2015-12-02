
using Davalor.Base.Messaging.Contracts;
namespace Davalor.PortalPaciente.Messages.Disclaimer
{
    public class RegisteredDisclaimer : BaseEvent
    {
        public RegisteredDisclaimer()
        {
            Topic = "Disclaimer";
        }
    }
}
