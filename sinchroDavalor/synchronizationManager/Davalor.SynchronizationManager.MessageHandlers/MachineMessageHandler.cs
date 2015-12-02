using Davalor.SAP.Messages.Machine;
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
    /// Message handler for Machine messages
    /// </summary>
    public class MachineMessageHandler  : GenericMessageHandler<MachineAggregate>
    {
        [ImportingConstructor]
        public MachineMessageHandler(
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
                this._handlers.Add(typeof(RegisteredMachine), this.Create);
                this._handlers.Add(typeof(ChangedMachine), this.Update);
                this._handlers.Add(typeof(UnregisteredMachine), this.Delete);
                StartListening();
            }
        }
    }
}
