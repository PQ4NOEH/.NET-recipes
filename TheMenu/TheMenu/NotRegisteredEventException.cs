using System;

namespace TheMenu.Core
{
    public class NotRegisteredEventException : Exception
    {
        public NotRegisteredEventException(string eventName, string entityName)
            : base(string.Format("The event {0} is not registered in the entity {1}.", eventName, entityName))
        {

        }
    }
}
