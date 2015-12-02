using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.Domain.MessageHandling
{
    /// <summary>
    /// Message decrypter interface
    /// </summary>
    public interface IMessageDecrypter
    {
        /// <summary>
        /// Decrypts the BaseEvent.Aggregate string to a T type
        /// </summary>
        /// <typeparam name="T">The TAggregate being transmited</typeparam>
        /// <param name="cryptedString">BaseEvent which contains an aggregate property</param>
        /// <returns>A instaqnce of T which all the transmited data</returns>
        T Decrypt<T>(NotNullable<BaseEvent> cryptedString);
    }
}
