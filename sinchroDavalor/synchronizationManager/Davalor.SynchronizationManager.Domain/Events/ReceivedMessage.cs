using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.Domain.Events
{
    /// <summary>
    /// The event of a receivedMessage
    /// </summary>
    public class ReceivedMessage : BaseEvent
    {
        /// <summary>
        /// The message who had received the message
        /// </summary>
        public string MessageHandler { get; set; }

    }
}
