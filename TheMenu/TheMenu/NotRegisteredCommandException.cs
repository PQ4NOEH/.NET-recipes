using System;

namespace TheMenu.Core
{
    public class NotRegisteredCommandException : Exception
    {
        public NotRegisteredCommandException(string commandName, string entityName)
            :base(string.Format("The command {0} is not registered in the entity {1}.", commandName, entityName))
        {

        }
    }
}
