using Davalor.SAP.Messages.Location;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Linq;
using System.ComponentModel.Composition;
using Davalor.SynchronizationManager.Domain.Events;
using System;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.MessageHandlers
{
    /// <summary>
    /// Message handler for Location messages
    /// </summary>
    public class LocationMessageHandler : GenericMessageHandler<LocationAggregate>
    {
        [ImportingConstructor]
        public LocationMessageHandler( 
            ISynchroRepositoryFactory repositoryFactory,
            IMessageDecrypter messageDecrypter,
            IHostConfiguration configuration,
            IServiceEvents serviceEvents)
            : base(repositoryFactory, messageDecrypter,serviceEvents)
        {
            var config = configuration.MessagesHandlers.FirstOrDefault(cfg => cfg.MessageHandlerName.Equals(_messageHandlerName, StringComparison.OrdinalIgnoreCase));
            if (config != null && config.SystemToSynchronize != null && config.SystemToSynchronize.Any())
            {
                _systemsToSyncronize.AddRange(config.SystemToSynchronize);
                this._handlers.Add(typeof(RegisteredLocation), this.Create);
                this._handlers.Add(typeof(ChangedLocation), this.Update);
                this._handlers.Add(typeof(UnregisteredLocation), this.Delete);
                StartListening();
            }
        }
    }
}
