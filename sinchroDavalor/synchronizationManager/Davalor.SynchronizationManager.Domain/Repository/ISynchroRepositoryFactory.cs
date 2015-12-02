
namespace Davalor.SynchronizationManager.Domain.Repository
{
    /// <summary>
    /// Represents a factory of repositories
    /// </summary>
    public interface ISynchroRepositoryFactory
    {
        /// <summary>
        /// Creates a repository for the given type of Aggregate
        /// </summary>
        /// <typeparam name="T">The aggregate type of the repository we want</typeparam>
        /// <returns>The repository</returns>
        ISynchroRepository<T> CreateDataService<T>()
            where T : class, ISynchroAggregateRoot;

        /// <summary>
        /// Creates a repository for the given type of Aggregate with the context for the given system
        /// </summary>
        /// <typeparam name="T">The aggregate type of the repository we want</typeparam>
        /// <param name="system">The target system</param>
        /// <returns>The repository</returns>
        ISynchroRepository<T> CreateDataService<T>(ESynchroSystem system)
            where T : class, ISynchroAggregateRoot;
    }
}
