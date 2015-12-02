﻿using Davalor.SAP.Messages.Service;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;

namespace Davalor.SynchronizationManager.MessageHandlers
{
    /// <summary>
    /// Message handler for Service messages
    /// </summary>
    public class ServiceMessageHandler : GenericMessageHandler<ServiceAggregate>
    {
        [ImportingConstructor]
        public ServiceMessageHandler(
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
                this._handlers.Add(typeof(RegisteredService), this.Create);
                this._handlers.Add(typeof(ChangedService), this.Update);
                this._handlers.Add(typeof(UnregisteredService), this.Delete);
                StartListening();
            }
        }
    }
}
