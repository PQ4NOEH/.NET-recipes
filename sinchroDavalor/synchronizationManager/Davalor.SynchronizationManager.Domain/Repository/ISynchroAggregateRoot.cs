using System;

namespace Davalor.SynchronizationManager.Domain.Repository
{
    /// <summary>
    /// Represents an AggregateRoot for the Synchronization context
    /// </summary>
    public interface ISynchroAggregateRoot
    {
        /// <summary>
        /// The Identificator of the aggregate
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// The DateTimeOffset of the last update of the aggregate
        /// </summary>
        DateTimeOffset TimeStamp { get; }
    }
}
