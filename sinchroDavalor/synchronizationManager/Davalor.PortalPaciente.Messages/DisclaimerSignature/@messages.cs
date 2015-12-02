
using Davalor.Base.Messaging.Contracts;
namespace Davalor.PortalPaciente.Messages.DisclaimerSignature
{
    public class SignedDisclaimer : BaseEvent
    {
        public SignedDisclaimer()
        {
            Topic = "DisclaimerSignature";
        }
    }
}
