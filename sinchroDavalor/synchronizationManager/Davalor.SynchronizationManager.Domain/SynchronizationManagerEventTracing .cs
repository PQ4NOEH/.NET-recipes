using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Domain
{
    [EventSource(Name = "SynchronizationManagerEventSourcing")]
    public class SynchronizationManagerEventTracing : EventSource
    {
        public static readonly Lazy<SynchronizationManagerEventTracing> Log = new Lazy<SynchronizationManagerEventTracing>(() => new SynchronizationManagerEventTracing());

        #region service level log method
        
        [Event(1, Level = EventLevel.Verbose)]
        public void Service_started(string instanceName, string machineName)
        {
            WriteEvent(1, instanceName, machineName);
        }
        [Event(2, Level = EventLevel.Verbose)]
        public void Service_stopped(string instanceName, string machineName)
        {
            WriteEvent(2, instanceName, machineName);
        }
        #endregion service level log method

        #region Message Level log method
        [Event(3, Level = EventLevel.Error)]
        public void Invalid_Message_received(string instanceName, string machineName, string topic, string messageType, string messageId, string invalidReason)
        {
            WriteEvent(3, instanceName, machineName, topic, messageType, messageId, invalidReason);
        }
        [Event(4, Level = EventLevel.Informational)]
        public void Message_received(string instanceName, string machineName, string messageType, string messageId, string messageHandlerName)
        {
            WriteEvent(4, instanceName, machineName, messageType, messageId, messageHandlerName);
        }
        [Event(5, Level = EventLevel.Error)]
        public void Message_processing_Exception(string instanceName, string machineName, string messageType, string messageId, string messageHandlerName, string errorMessage, string errorStackTrace)
        {
            WriteEvent(5, instanceName, machineName, messageType, messageId, messageHandlerName, errorMessage, errorStackTrace);
        }
        [Event(6, Level = EventLevel.Informational)]
        public void Message_Processed(string instanceName, string machineName, string messageType, string messageId, string messageHandlerName)
        {
            WriteEvent(6, instanceName, machineName, messageType, messageId, messageHandlerName);
        }
        #endregion
    }
}
