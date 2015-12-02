using Davalor.Base.Library.Guards;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Subjects;

namespace Davalor.SynchronizationManager.Domain.Events
{
    /// <summary>
    /// Implentation of IServiceEvents 
    /// <seealso cref="IServiceEvents"/>
    /// </summary>
    [Export(typeof(IServiceEvents))]
    public class ServiceEvents : IServiceEvents
    {
        ReplaySubject<IncommingEvent> _incommingEventSequence = new ReplaySubject<IncommingEvent>(10);
        public void AddIncommingEvent(NotNullable<IncommingEvent> @event)
        {
            _incommingEventSequence.OnNext(@event);
        }

        public IObservable<IncommingEvent> IncommingEventSequence
        {
            get { return _incommingEventSequence; }
        }

        ReplaySubject<ReceivedMessage> _receivedMessageEventSequence = new ReplaySubject<ReceivedMessage>(10);
        public void AddReceivedMessageEvent(NotNullable<ReceivedMessage> @event)
        {
            _receivedMessageEventSequence.OnNext(@event);
        }

        public IObservable<ReceivedMessage> ReceivedMessageEventSequence
        {
            get { return _receivedMessageEventSequence; }
        }

        ReplaySubject<ReceivedInvalidMessage> _receivedInvalidMessageEventSequence = new ReplaySubject<ReceivedInvalidMessage>(10);
        public void AddReceivedInvalidMessageEvent(NotNullable<ReceivedInvalidMessage> @event)
        {
            _receivedInvalidMessageEventSequence.OnNext(@event);
        }

        public IObservable<ReceivedInvalidMessage> ReceivedInvalidMessageEventSequence
        {
            get { return _receivedInvalidMessageEventSequence; }
        }

        ReplaySubject<ReceivedMessage> _procesedMessageEventSequence = new ReplaySubject<ReceivedMessage>(10);
        public void AddProcesedMessageEvent(NotNullable<ReceivedMessage> @event)
        {
            _procesedMessageEventSequence.OnNext(@event);
        }

        public IObservable<ReceivedMessage> ProcesedMessageEventSequence
        {
            get { return _procesedMessageEventSequence; }
        }

        ReplaySubject<ProcesedMessageException> _procesedMessageExceptionSequence = new ReplaySubject<ProcesedMessageException>(10);
        public void AddProcesedMessageException(NotNullable<ProcesedMessageException> @event)
        {
            _procesedMessageExceptionSequence.OnNext(@event);
        }

        public IObservable<ProcesedMessageException> ProcesedMessageExceptionSequence
        {
            get { return _procesedMessageExceptionSequence; }
        }
    }
}
