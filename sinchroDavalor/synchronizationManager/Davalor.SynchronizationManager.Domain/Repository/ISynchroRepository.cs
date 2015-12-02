using System;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Domain.Repository
{
    /// <summary>
    /// Represents a repository for the synchronization
    /// </summary>
    /// <typeparam name="T">The Aggregate it manages</typeparam>
    public interface ISynchroRepository<T> : IDisposable where T : ISynchroAggregateRoot
    {

        /// <summary>
        /// Saves the aggregate in the context
        /// </summary>
        /// <param name="aggregate">The aggregate to be saved</param>
        /// <returns>Task for async management</returns>
        Task Save(T aggregate);

        /// <summary>
        /// Updates the aggregate in the context
        /// </summary>
        /// <param name="aggregate">The aggregate to be updated</param>
        /// <returns>Task for async management</returns>
        Task Update(T aggregate);

        /// <summary>
        /// Removes from the context the aggregate with the provided Id
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate which want to be removed from the context</param>
        /// <returns>Task for async management</returns>
        Task Delete(Guid aggregateId);

        /// <summary>
        /// Indicates if an aggregate with the provided id exists on the context
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate we want to know if exists</param>
        /// <returns>Task for async management</returns>
        Task<bool> Exists(Guid aggregateId);

        /// <summary>
        /// Indicates if an aggregate with the provided id has been changed since the provided date
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate</param>
        /// <param name="changedTime">The date from which we want to know if any change has occur</param>
        /// <returns>true/false</returns>
        Task<bool> HasBeenChanged(Guid aggregateId, DateTimeOffset changedTime);
    }
}
