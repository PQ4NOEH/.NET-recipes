using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository
{
    /// <summary>
    /// Base repository for all repositories
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate is going to manage</typeparam>
    public abstract class GenericDataService<TAggregate> : ISynchroRepository<TAggregate>
       where TAggregate : class, ISynchroAggregateRoot
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TAggregate> _dbSet;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dbContext">The dbContext is going to work against</param>
        public GenericDataService(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TAggregate>();
        }
        /// <summary>
        /// Saves the given aggregate to the underlaying database context
        /// </summary>
        /// <param name="aggregate">The aggregate</param>
        /// <returns>Task for async management</returns>
        public virtual async Task Save(TAggregate aggregate)
        {
            _dbSet.Add(aggregate);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task Update(TAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified; 
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Removes the aggregate with the given Id from the underlaying database context
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate we want to remove from the context</param>
        /// <returns>Task for async management</returns>
        public virtual async Task Delete(Guid aggregateId)
        {
            var match = await _dbSet.FindAsync(aggregateId);
            if (match != null)
            {
                _dbSet.Remove(match);
                await _dbContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Indicate if the aggregate with the given Id exists in the context
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate we want to know if exists</param>
        /// <returns>True if it does exist, otherwise fase</returns>
        public virtual async Task<bool> Exists(Guid aggregateId)
        {
            return await _dbSet.FindAsync(aggregateId) != null;
        }
        /// <summary>
        ///  Indicate if the aggregate with the given Id has changed since the provided datetime
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate we want to know if it has changed</param>
        /// <param name="changedTime">The date we want to know if the aggregate has changed since then.</param>
        /// <returns></returns>
        public virtual async Task<bool> HasBeenChanged(Guid aggregateId, DateTimeOffset changedTime)
        {
            var firstMatch = await _dbSet.FirstOrDefaultAsync(aRoot => aRoot.Id == aggregateId && aRoot.TimeStamp > changedTime);
            return firstMatch != null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
    }
}
