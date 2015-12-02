
using Davalor.Base.Library.Guards;
namespace Davalor.SynchronizationManager.Domain.Listeners
{
    /// <summary>
    /// Listener for kafka topics 
    /// </summary>
    public interface IKafkaHostListener
    {
        /// <summary>
        /// Allows to hook the system to topics in kafka
        /// </summary>
        /// <param name="topicName">The kafka Wanted to be listened</param>
        void ListenToTopic(NotNullOrWhiteSpaceString topicName);
    }
}
