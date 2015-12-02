using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Listener.Kafka;
using System;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.Base.Messaging.Kafka;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Kafka.Contracts;
using Davalor.Base.Library.Serialization;
using Davalor.SynchronizationManager.Domain;

namespace Davalor.SynchronizationManager.Host
{
    /// <summary>
    /// Load application services,and their dependencies
    /// </summary>
    public class BootStrapper
    {
        readonly CompositionContainer _container;
        HostLogger _logger;
        public BootStrapper()
        {
            var catalog = new AggregateCatalog();
            var directoryCatalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "Davalor.*.dll");
            catalog.Catalogs.Add(directoryCatalog);
            _container = new CompositionContainer(catalog);
        }
       
        /// <summary>
        /// initializates the host services
        /// </summary>
        public void StartServices()
        {

            foreach (var partDefiniton in _container.Catalog.Parts)
            {
                var part = partDefiniton.CreatePart();
                _container.SatisfyImportsOnce(part);
                part.Activate();
            }

        }

        public T GetService<T>()
        {
            return (T) _container.GetExports(typeof(T), null, null).FirstOrDefault().Value;
        }
        /// <summary>
        /// Starts kafkaListener
        /// </summary>
        public void StartKafkaListener()
        {
            var configuration = _container.GetExports(typeof(IHostConfiguration), null, null).FirstOrDefault().Value as IHostConfiguration;
            var serviceEvents = _container.GetExports(typeof(IServiceEvents), null, null).FirstOrDefault().Value as IServiceEvents;
            var KafkaConsumerFactory = new KafkaConsumerFactory(new NotNullable<IKafkaConfiguration>(configuration.kafkaConfiguration));
            var listenerFactory = new KafkaListenerFactory(KafkaConsumerFactory);
            
            var kafkaHostListener = new KafkaHostListener(
                serviceEvents,
                listenerFactory, 
                new BinaryJsonSerializer());

            configuration
                .KafkaTopicsToListen
                .ToList()
                .ForEach(s => kafkaHostListener.ListenToTopic(s));

        }

        public void StartLogger()
        {
            if(_logger == null)
            {
                var config = GetService<IHostConfiguration>();
                var serviceEvents = GetService<IServiceEvents>();
                _logger = new HostLogger(
                    new NotNullable<IServiceEvents>(serviceEvents),
                    SynchronizationManagerEventTracing.Log.Value,
                    new NotNullable<IHostConfiguration>(config)
                 );
            }
            _logger.LogAppStarted();
        }
    }
}
