using Davalor.SAP.Messages.Machine;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.Machine
{
    public class MachineRepository : GenericDataService<MachineAggregate>
    {
        public MachineRepository(DbContext context) : base(context) { }

        public override async Task Delete(Guid aggregateId)
        {
            var aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                aggregate.MachinePrinter.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }

        public override async Task Update(MachineAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate.MachineGroup).State = EntityState.Modified;
            aggregate.MachinePrinter.ToList().ForEach(p => _dbContext.Entry(p).State = EntityState.Modified);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
