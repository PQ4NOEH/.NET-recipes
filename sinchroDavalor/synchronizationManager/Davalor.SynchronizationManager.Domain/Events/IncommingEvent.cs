using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.Domain.Events
{
    /// <summary>
    /// An event listened by the host
    /// </summary>
    public class IncommingEvent
    {
        /// <summary>
        /// The message deserialized in form of a baseEvent
        /// </summary>
        public BaseEvent @event {get; set;}
        /// <summary>
        /// The message as it's initially listened
        /// </summary>
        public byte[] RawData { get; set; }
    }
}
