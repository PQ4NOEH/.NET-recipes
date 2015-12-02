using Davalor.SAP.Messages.PaymentGateway;
using Davalor.SynchronizationManager.Domain;
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
    /// Message handler for PaymentGateway messages
    /// </summary>
    public class PaymentGatewayMessageHandler : GenericMessageHandler<GatewayAggregate>
    {
        [ImportingConstructor]
        public PaymentGatewayMessageHandler(
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
                this._handlers.Add(typeof(RegisteredPaymentGateway), this.Create);
                this._handlers.Add(typeof(ChangedPaymentGateway), this.Update);
                this._handlers.Add(typeof(UnregisteredPaymentGateway), this.Delete);
                StartListening();
            }
        }
    }
}
