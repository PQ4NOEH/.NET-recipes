using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Currency
{
    public class RegisteredCurrency : BaseEvent 
    {
        public RegisteredCurrency()
        {
            Topic = "Currency";
        }
    }
    public class ChangedCurrency : BaseEvent 
    {
        public ChangedCurrency()
        {
            Topic = "Currency";
        }
    }
    public class UnregisteredCurrency : BaseEvent 
    {
        public UnregisteredCurrency()
        {
            Topic = "Currency";
        }
    }

}
