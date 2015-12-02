using Davalor.Base.Library.Serialization;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Davalor.SynchronizationManager.Host
{
    /// <summary>
    /// HostConfiguration
    /// </summary>
    [Export(typeof(IHostConfiguration))]
    public class HostConfiguration : IHostConfiguration
    {
        /// <summary>
        /// Constructor. Loads the file "HostConfiguration.json" and deserializates it. 
        /// </summary>
        [ImportingConstructor]
        public HostConfiguration()
        {
            var configuration = File.ReadAllText("configurationApp.json");
            new JsonSerializer().PopulateObject<HostConfiguration>(configuration, this);
        }
        /// <summary>
        /// The list of kafka topics we want to listen to
        /// </summary>
        public List<string> KafkaTopicsToListen
        {
            get;
            set;
        }
        /// <summary>
        /// The configuration of the kafka client <seealso cref="Davalor.Framework.Core.Messaging.Kafka.KafkaConfiguration"/>
        /// </summary>
        public KafkaConfiguration kafkaConfiguration
        {
            get;
            set;
        }
        /// <summary>
        /// The connection string for the PortalPaciente database
        /// </summary>
        public string PortalPacienteConnectionString
        {
            get;
            set;
        }
        /// <summary>
        /// The connection string for the  VisionLocal database
        /// </summary>
        public string VisionLocalConnectionString
        {
            get;
            set;
        }
        List<string> _brokenRules = new List<string>();
        /// <summary>
        /// Collection of configuration broken rules <seealso cref="Davalor.Framework.Core.Configuration.IHostConfiguration"/>
        /// </summary>
        public IEnumerable<string> BrokenRules
        {
            get { return _brokenRules; }
        }
        /// <summary>
        /// Indicates if the current state of the configuration class is valid.
        /// </summary>
        /// <returns>true if the configuration is valid, false otherwise</returns>
        public bool IsValid()
        {
            _brokenRules = new List<string>();
            if(kafkaConfiguration == null)
            {
                _brokenRules.Add("The kafkaConfiguration can not be null.");
            }
            else if(kafkaConfiguration.Uri.Count() == 0)
            {
                _brokenRules.Add("kafkaConfiguration has to have a minimum of one valid adrress.");
            }
            if(KafkaTopicsToListen == null || KafkaTopicsToListen.Count == 0)
            {
                _brokenRules.Add("A minimum of one topic has to be configured.");
            }
            if(string.IsNullOrWhiteSpace(PortalPacienteConnectionString))
            {
                _brokenRules.Add("PortalPacienteConnectionString has to have value.");
            }
            if (string.IsNullOrWhiteSpace(VisionLocalConnectionString))
            {
                _brokenRules.Add("VisionLocalConnectionString has to have value.");
            }
            return _brokenRules.Count == 0;
        }

        /// <summary>
        /// Configuration of the messageHandlers we want to be active
        /// </summary>
       
        public List<MessageHandlerConfiguration> MessagesHandlers
        {
            get;
            set;
        }

        public string HostPasswordFilePath { get; set; }

        public string InstanceName
        {
            get;
            set;
        }

        public string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }
    }
}
