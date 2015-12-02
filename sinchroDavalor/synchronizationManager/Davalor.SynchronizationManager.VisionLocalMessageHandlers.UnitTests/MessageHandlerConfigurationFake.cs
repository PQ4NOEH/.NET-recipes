using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public class MessageHandlerConfigurationFake : MessageHandlerConfiguration
    {
        public MessageHandlerConfigurationFake(string handlerName, IEnumerable<ESynchroSystem> systemToSynchronize)
        {
            MessageHandlerName = handlerName;
            SystemToSynchronize = (systemToSynchronize == null) ? new List<ESynchroSystem>() : new List<ESynchroSystem>(systemToSynchronize);
        }
    }
}
