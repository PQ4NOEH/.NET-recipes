using Davalor.Base.Library.Guards;
using System;

namespace Davalor.SynchronizationManager.Domain.Events
{
    /// <summary>
    /// Contains information of an exception occurred during the procesing of a message
    /// </summary>
    public class ProcesedMessageException : Exception
    {
        /// <summary>
        /// The EventId of the message which was being processed
        /// </summary>
        public Guid EventID { get; private set; }
        /// <summary>
        /// The MessageType of the message which was being processed
        /// </summary>
        public string MessageType { get; private set; }
        /// <summary>
        /// The Handler which tried to process the message
        /// </summary>
        public string MessageHandler { get; private set; } 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventID">The EventId of the message which was being processed</param>
        /// <param name="messageType">The MessageType of the message which was being processed</param>
        /// <param name="messageHandler">The Handler which tried to process the message</param>
        /// <param name="message">The exception message</param>
        /// <param name="exception">The catched exception</param>
        public ProcesedMessageException(
            Guid eventID, 
            NotNullOrWhiteSpaceString messageType, 
            NotNullOrWhiteSpaceString messageHandler,
            NotNullOrWhiteSpaceString message,
            NotNullable<Exception> exception)
            :base(message, exception)
        {
            EventID = eventID;
            MessageType = messageType;
            MessageHandler = messageHandler;
        }
    }
}
