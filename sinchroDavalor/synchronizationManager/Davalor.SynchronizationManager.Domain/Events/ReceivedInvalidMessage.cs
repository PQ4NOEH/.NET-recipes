using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.Domain.Events
{
    /// <summary>
    /// Represents a message which is invalid because of any of the defined reasons in EInvalidMessageReason type
    /// </summary>
    public class ReceivedInvalidMessage : BaseEvent
    {
        /// <summary>
        /// The handler who signaled the message as invalid
        /// </summary>
        public string MessageHandler { get; set; }
    }
}
