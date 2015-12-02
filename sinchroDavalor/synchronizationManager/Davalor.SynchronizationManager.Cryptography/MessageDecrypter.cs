using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Contracts;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using System.ComponentModel.Composition;

namespace Davalor.SynchronizationManager.Cryptography
{
    /// <summary>
    /// A class capable of decrypt a string into a given type
    /// </summary>
    [Export(typeof(IMessageDecrypter))]
    public class MessageDecrypter : IMessageDecrypter
    {
        readonly IHostPasswordProvider _passwordProvider;
        readonly IStringSerializer _serializer;
        readonly ICryptoManager _cryptoManager;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="passwordProvider">An instance capable of generate password for a given machine</param>
        /// <param name="serializer">A string serializer</param>
        /// <param name="cryptoManager">A cryptography manager</param>
        [ImportingConstructor]
        public MessageDecrypter(IHostPasswordProvider passwordProvider, IStringSerializer serializer, ICryptoManager cryptoManager)
        {
            _passwordProvider = passwordProvider;
            _serializer = serializer;
            _cryptoManager = cryptoManager;
        }
        /// <summary>
        /// Decrypts the property Aggregate of a baseEvent to the given type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="message">The message which has an Aggregate property we want to decrypt</param>
        /// <returns>The instance of T with decrypted data</returns>
        public T Decrypt<T>(NotNullable<BaseEvent> message)
        {
            var password = _passwordProvider.PasswordFor(message.Value.MessageOriginator);
            var aggregate = _cryptoManager.Decrypt(message.Value.Aggregate, password);
            return _serializer.Deserialize<T>(aggregate);
        }
    }
}
