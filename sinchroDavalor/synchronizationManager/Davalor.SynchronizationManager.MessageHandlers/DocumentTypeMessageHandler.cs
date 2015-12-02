using Davalor.SAP.Messages.DocumentType;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Linq;
using System.ComponentModel.Composition;

using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using System;

namespace Davalor.SynchronizationManager.MessageHandlers
{
    /// <summary>
    /// Message handler for DocumentType messages
    /// </summary>
    public class DocumentTypeMessageHandler  : GenericMessageHandler<DocumentTypeAggregate>
    {
        [ImportingConstructor]
        public DocumentTypeMessageHandler(
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
                this._handlers.Add(typeof(RegisteredDocumentType), this.Create);
                this._handlers.Add(typeof(ChangedDocumentType), this.Update);
                this._handlers.Add(typeof(UnregisteredDocumentType), this.Delete);
                StartListening();
            }
        }
    }
}
