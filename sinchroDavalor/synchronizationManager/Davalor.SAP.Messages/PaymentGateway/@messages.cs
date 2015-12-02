
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.PaymentGateway
{
    public class RegisteredPaymentGateway : BaseEvent
    {
        public RegisteredPaymentGateway()
        {
            Topic = "PaymentGateway";
        }
    }
    public class ChangedPaymentGateway : BaseEvent
    {
        public ChangedPaymentGateway()
        {
            Topic = "PaymentGateway";
        }
    }
    public class UnregisteredPaymentGateway : BaseEvent
    {
        public UnregisteredPaymentGateway()
        {
            Topic = "PaymentGateway";
        }
    }
}
