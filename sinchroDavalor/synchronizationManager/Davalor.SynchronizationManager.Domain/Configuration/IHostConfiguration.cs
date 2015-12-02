
using Davalor.Base.Contract.Library;
using Davalor.Base.Messaging.Kafka.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Davalor.SynchronizationManager.Domain.Configuration
{
    /// <summary>
    /// Signature of the host configuration
    /// </summary>
    public interface IHostConfiguration : IValidable
    {
        string InstanceName { get; }
        string MachineName { get; }
        List<string> KafkaTopicsToListen { get; }
        KafkaConfiguration kafkaConfiguration { get; }
        string PortalPacienteConnectionString { get; }
        string VisionLocalConnectionString { get; }
        List<MessageHandlerConfiguration> MessagesHandlers { get; }
        string HostPasswordFilePath { get; }
    }
    /// <summary>
    /// Signature of the configuration of one messageHandler
    /// </summary>
    /// 
    public interface IMessageHandlerConfiguration
    {
        string MessageHandlerName { get; set; }
        List<ESynchroSystem> SystemToSynchronize{ get; set; }
    }
    public class MessageHandlerConfiguration : IMessageHandlerConfiguration
    {
        /// <summary>
        /// The name of the handler we want to configure. It Has to match the IServiceMessageHandler class name.
        /// </summary>
        public string MessageHandlerName { get; set; }
        /// <summary>
        /// The Systems the messageHandler has to configure.
        /// </summary>
        public List<ESynchroSystem> SystemToSynchronize { get; set; }

        public List<string> TopicsToListen { get; set; }
    }

    public class KafkaConfiguration: IKafkaConfiguration
    {

        public List<string> TopicsToListen { get; set; }
        public string[] Address
        {
            get;
            set;
        }

        public string ConsumerGroup
        {
            get;
            set;
        }

        public Uri[] Uri
        {
            get { return Address.Select(a => new Uri(a)).ToArray(); }
        }
    }
}
