using Davalor.SAP.Messages.Region;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Davalor.SynchronizationManager.MessageHandlers
{
    /// <summary>
    /// Message handler for Region messages
    /// </summary>
    public class RegionMessageHandler : GenericMessageHandler<RegionAggregate>
    {
        [ImportingConstructor]
        public RegionMessageHandler(
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
                this._handlers.Add(typeof(RegisteredRegion), this.Create);
                this._handlers.Add(typeof(ChangedRegion), this.Update);
                this._handlers.Add(typeof(UnregisteredRegion), this.Delete);
                StartListening();
            }
        }
    }
}
