
namespace Davalor.SynchronizationManager.Domain.MessageHandling
{
    /// <summary>
    /// Allows to get the password for a given machine
    /// </summary>
    public interface IHostPasswordProvider
    {
        /// <summary>
        /// Gets the password corresponding to a given machine
        /// </summary>
        /// <param name="machineName">The machine we want the password</param>
        /// <returns>The password nedded to decrypt the received message</returns>
        string PasswordFor(string machineName);
    }
}
