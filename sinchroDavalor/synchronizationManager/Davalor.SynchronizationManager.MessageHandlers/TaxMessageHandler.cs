using Davalor.SAP.Messages.Tax;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using System.Linq;
using System.ComponentModel.Composition;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using System;

namespace Davalor.SynchronizationManager.MessageHandlers
{
    /// <summary>
    /// Message handler for Tax messages
    /// </summary>
    public class TaxMessageHandler : GenericMessageHandler<TaxAggregate>
    {
        [ImportingConstructor]
        public TaxMessageHandler(
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
                this._handlers.Add(typeof(RegisteredTax), this.Create);
                this._handlers.Add(typeof(ChangedTax), this.Update);
                this._handlers.Add(typeof(UnregisteredTax), this.Delete);
                StartListening();
            }
        }
    }
}

