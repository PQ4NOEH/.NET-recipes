using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Domain.MessageHandling
{
    /// <summary>
    /// Base class for the messageHandlers
    /// </summary>
    /// <typeparam name="T">The aggregateType the handler tackles.</typeparam>
    public abstract class GenericMessageHandler<T> : IServiceMessageHandler where T : class, ISynchroAggregateRoot 
    {
        protected readonly ISynchroRepositoryFactory _repositoryFactory;
        protected readonly List<ESynchroSystem> _systemsToSyncronize = new List<ESynchroSystem>();
        protected readonly IMessageDecrypter _messageDecrypter;
        protected readonly Dictionary<Type, Action<T>> _handlers = new Dictionary<Type, Action<T>>();
        readonly IServiceEvents _serviceEvents;
        protected readonly string _messageHandlerName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryFactory">Factory of repositories</param>
        /// <param name="messageDecrypter">string Decrypter</param>
        /// <param name="serviceEvents">IserviceEvent instance</param>
        /// <param name="serializer">Binary serialization service</param>
        public GenericMessageHandler(
            ISynchroRepositoryFactory repositoryFactory,
            IMessageDecrypter messageDecrypter,
            IServiceEvents serviceEvents)
        {
            _messageHandlerName = this.GetType().Name;
            _repositoryFactory = repositoryFactory;
            _messageDecrypter = messageDecrypter;
            _serviceEvents = serviceEvents;
        }
        /// <summary>
        /// Starts subscriptions to defined message types 
        /// </summary>
        protected virtual void StartListening()
        {
            foreach (var configuredHandler in _handlers)
            {
                _serviceEvents.IncommingEventSequence
                    .Where(i => i.@event.MessageType.Equals(configuredHandler.Key.Name, StringComparison.OrdinalIgnoreCase))
                    .Subscribe(i => Handle(i, configuredHandler.Value));
            }
        }
        /// <summary>
        /// Handles an IncommingEvent
        /// </summary>
        /// <param name="event">The IncommingEvent</param>
        /// <param name="eventHandler">The action configured to handled the event</param>
        protected virtual void Handle(IncommingEvent @event, Action<T> eventHandler)
        {
            var receivedMessage = new ReceivedMessage
            {
                EventID = @event.@event.EventID,
                MessageType = @event.@event.MessageType,
                Topic = @event.@event.Topic,
                MessageHandler = _messageHandlerName
            };
            _serviceEvents.AddReceivedMessageEvent(receivedMessage);//emits a receivedMessage event
            try
            {
                var aggregate = _messageDecrypter.Decrypt<T>(@event.@event);
                eventHandler(aggregate);
                _serviceEvents.AddProcesedMessageEvent(receivedMessage);//If everything goes fine emits a ProcesedMessage event
            }
            catch (Exception ex)
            {
                _serviceEvents.AddProcesedMessageException(new ProcesedMessageException(//If any exception happens during message procesing emits a ProcesedMessageException event
                    @event.@event.EventID,
                    @event.@event.MessageType,
                    _messageHandlerName,
                    ex.Message,
                    ex
                    ));
            }
        }
        /// <summary>
        /// Default implementation of a create T task
        /// </summary>
        /// <param name="aggregate">The Aggregate which it is going to be created</param>
        protected virtual void Create(T aggregate)
        {

            _systemsToSyncronize.ForEach(eSystem => // we execute the task on each configured ESynchroSystem
                {
                    using (var repository = _repositoryFactory.CreateDataService<T>(eSystem))
                    {
                        repository.Save(aggregate).Wait();
                    }
                });
        }
        /// <summary>
        /// Default implementation of a update T task
        /// </summary>
        /// <param name="aggregate">The Aggregate which it is going to be updated</param>
        protected virtual void Update(T aggregate)
        {
            _systemsToSyncronize.ForEach(eSystem =>// we execute the task on each configured ESynchroSystem
            {
                using (var repository = _repositoryFactory.CreateDataService<T>(eSystem))
                {
                    repository.Update(aggregate).Wait();
                }
            });
        }
        /// <summary>
        /// Default implementation of a Delete T task
        /// </summary>
        /// <param name="aggregate">The Aggregate which it is going to be removed</param>
        protected virtual void Delete(T aggregate)
        {
            _systemsToSyncronize.ForEach(eSystem =>// we execute the task on each configured ESynchroSystem
            {
                using (var repository = _repositoryFactory.CreateDataService<T>(eSystem))
                {
                    repository.Delete(aggregate.Id).Wait();
                }
            });
        }
    }
}
