using Davalor.Base.Library.Guards;
using Davalor.Base.Security.Contracts;
using Davalor.Base.Security.Encryption;
using System.ComponentModel.Composition;
using System.Security.Cryptography;

namespace Davalor.SynchronizationManager.Cryptography
{
    /// <summary>
    /// Service for encryption / decryption. It is a wrapper of CryptoManager <see cref="Davalor.Framework.Cryptography.CryptoManager"/>
    /// </summary>
    [Export(typeof(ICryptoManager))]
    public class CryptoManagerService : ICryptoManager
    {
        readonly ICryptoManager _adaptee;

        [ImportingConstructor]
        public CryptoManagerService()
        {
            _adaptee = new CryptoManager();
        }
        public string Decrypt<T>(NotNullOrWhiteSpaceString value, NotNullOrWhiteSpaceString password) where T : SymmetricAlgorithm, new()
        {
            return _adaptee.Decrypt<T>(value, password);
        }

        public string Decrypt(NotNullOrWhiteSpaceString value, NotNullOrWhiteSpaceString pwd)
        {
            return _adaptee.Decrypt(value, pwd);
        }

        public string Encrypt<T>(NotNullOrWhiteSpaceString value, NotNullOrWhiteSpaceString password) where T : System.Security.Cryptography.SymmetricAlgorithm, new()
        {
            return _adaptee.Encrypt<T>(value, password);
        }

        public string Encrypt(NotNullOrWhiteSpaceString value, NotNullOrWhiteSpaceString pwd)
        {
            return _adaptee.Encrypt(value, pwd);
        }
    }
}
