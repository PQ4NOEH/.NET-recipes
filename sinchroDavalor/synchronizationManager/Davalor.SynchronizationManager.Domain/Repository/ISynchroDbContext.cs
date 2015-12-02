using Davalor.SynchronizationManager.Domain;

namespace Davalor.SynchronizationManager.Domain.Repository
{
    /// <summary>
    /// A Database context for the Synchronization
    /// </summary>
    /// <typeparam name="T">The T Aggregate it operates on</typeparam>
    public interface ISynchroDbContext<T>
    {
        /// <summary>
        /// The system the context operates on
        /// </summary>
        ESynchroSystem SynchroSystem { get; }
    }
}
