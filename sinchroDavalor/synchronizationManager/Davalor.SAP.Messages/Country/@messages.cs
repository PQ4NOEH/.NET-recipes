using Davalor.Base.Messaging.Contracts;
using System;

namespace Davalor.SAP.Messages.Country
{
    public class RegisteredCountry : BaseEvent 
    {
        public RegisteredCountry()
        {
            Topic = "Country";
        }
    }
    public class ChangedCountry : BaseEvent  
    {
        public ChangedCountry()
        {
            Topic = "Country";
        }
    }
    public class UnregisteredCountry : BaseEvent  
    {
        public UnregisteredCountry()
        {
            Topic = "Country";
        }
    }
}
