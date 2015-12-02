using System;
using System.Diagnostics.Tracing;

namespace Davalor.MomProxy.Domain
{
    [EventSource(Name = "MomProxyEventSourcing")]
    public class MomProxyEventTracing : EventSource
    {
        public static readonly Lazy<MomProxyEventTracing> Log = new Lazy<MomProxyEventTracing>(() => new MomProxyEventTracing());

        #region service level log method
        
        [Event(1, Level = EventLevel.Verbose)]
        public void Service_started(string applicationName, string machineName)
        {
            WriteEvent(1, applicationName, machineName);
        }
        
        [Event(2, Level = EventLevel.Verbose)]
        public void Service_stopped(string applicationName, string machineName)
        {
            WriteEvent(2, applicationName, machineName);
        }
        #endregion service level log method

        #region Message Level log method

        [Event(3, Level = EventLevel.Informational)]
        public void Message_Generated(string applicationName, string machineName, string topic, string messageId)
        {
            WriteEvent(3, applicationName, machineName);
        }
        [Event(4, Level = EventLevel.Informational)]
        public void Message_received(string applicationName, string machineName, string topic, string messageId, string messageHandlerName)
        {
            WriteEvent(4, applicationName, machineName, topic, messageId, messageHandlerName);
        }

        [Event(5, Level = EventLevel.Informational)]
        public void Message_saved_in_local_storage(string applicationName, string machineName, string messageId)
        {
            WriteEvent(5, applicationName, machineName, messageId);
        }

        [Event(6, Level = EventLevel.Informational)]
        public void Message_send_to_MOM(string applicationName, string machineName, string messageId)
        {
            WriteEvent(6, applicationName, machineName, messageId);
        }

        [Event(7, Level = EventLevel.Informational)]
        public void Message_deleted_from_local_storage(string applicationName, string machineName, string messageId)
        {
            WriteEvent(7, applicationName, machineName, messageId);
        }

        [Event(8, Level = EventLevel.Error)]
        public void Invalid_Message_received(string applicationName, string machineName, string topic, string messageId, string invalidReason)
        {
            WriteEvent(8, applicationName, machineName, topic, messageId, invalidReason);
        }

        #endregion
    }
}
