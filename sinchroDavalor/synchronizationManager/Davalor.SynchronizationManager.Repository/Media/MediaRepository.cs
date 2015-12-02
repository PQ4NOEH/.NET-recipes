using Davalor.SAP.Messages.Media;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace Davalor.SynchronizationManager.Repository.Media
{
    public class MediaRepository : GenericDataService<MediaAggregate>
    {
        public MediaRepository(DbContext context) : base(context) { }

        public override async Task Update(MediaAggregate aggregate)
        {
            _dbSet.Attach(aggregate);

            _dbContext.Entry(aggregate).State = EntityState.Modified;
            aggregate.MediaDeviceGroup.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Modified);
            aggregate.MediaMachine.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Modified);
            aggregate.MediaServiceLevel.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Modified);

            await _dbContext.SaveChangesAsync();
        }

        public override async Task Delete(System.Guid aggregateId)
        {
            var aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                aggregate.MediaDeviceGroup.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                aggregate.MediaMachine.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                aggregate.MediaServiceLevel.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
