
using Davalor.Base.Library.Guards;
using System;

namespace Davalor.SynchronizationManager.Domain.Events
{
   /// <summary>
   /// Sygnature of the events produced within the host boundaries
   /// </summary>
    public interface IServiceEvents
    {
        /// <summary>
        /// Adds to the system a listened event
        /// </summary>
        /// <param name="event">listened event</param>
        void AddIncommingEvent(NotNullable<IncommingEvent> @event);
        /// <summary>
        /// The secuence of listened message
        /// </summary>
        IObservable<IncommingEvent> IncommingEventSequence{get;}
        /// <summary>
        /// Adds to the system a ReceivedMessage event
        /// </summary>
        /// <param name="event">the event of type ReceivedMessage</param>
        void AddReceivedMessageEvent(NotNullable<ReceivedMessage> @event);
        /// <summary>
        /// The secuence of ReceivedMessage events happened
        /// </summary>
        IObservable<ReceivedMessage> ReceivedMessageEventSequence { get; }
        /// <summary>
        /// Adds to the system a ReceivedInvalidMessage event
        /// </summary>
        /// <param name="event">the event of type ReceivedInvalidMessage</param>
        void AddReceivedInvalidMessageEvent(NotNullable<ReceivedInvalidMessage> @event);
        /// <summary>
        /// The secuence of ReceivedInvalidMessage events happened
        /// </summary>
        IObservable<ReceivedInvalidMessage> ReceivedInvalidMessageEventSequence { get; }
        /// <summary>
        /// Adds to the system a ProcesedMessage event
        /// </summary>
        /// <param name="event">the event of type ProcesedMessage</param>
        void AddProcesedMessageEvent(NotNullable<ReceivedMessage> @event);
        /// <summary>
        /// The secuence of ProcesedMessage events happened
        /// </summary>
        IObservable<ReceivedMessage> ProcesedMessageEventSequence { get; }
        /// <summary>
        /// Adds to the system a ProcesedMessageException event
        /// </summary>
        /// <param name="event">the event of type ProcesedMessageException</param>
        void AddProcesedMessageException(NotNullable<ProcesedMessageException> @event);
        /// <summary>
        /// The secuence of ProcesedMessageException events happened
        /// </summary>
        IObservable<ProcesedMessageException> ProcesedMessageExceptionSequence { get; }
    }
}
