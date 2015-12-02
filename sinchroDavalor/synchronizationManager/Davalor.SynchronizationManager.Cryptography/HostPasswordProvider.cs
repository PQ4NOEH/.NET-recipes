using Davalor.Base.Security.Encryption;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using System.ComponentModel.Composition;

namespace Davalor.SynchronizationManager.Cryptography
{
    /// <summary>
    /// class to generate the password key for decrypt a message from a given machine.
    /// </summary>
    [Export(typeof(IHostPasswordProvider))]
    public class HostPasswordProvider : IHostPasswordProvider
    {
        readonly HostPasswordConfiguration _configuration;
        [ImportingConstructor]
        public HostPasswordProvider(IHostConfiguration configuration)
        {
            _configuration = new HostPasswordConfiguration();
            _configuration.LoadFromFile(configuration.HostPasswordFilePath);
        }
        /// <summary>
        /// Generates the password corresponding top the given machine
        /// </summary>
        /// <param name="machine">The machine we want the password</param>
        /// <returns>The password</returns>
        public string PasswordFor(string machine)
        {
            return _configuration[machine].ToString();
        }
    }
}
