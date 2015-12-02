using Davalor.PortalPaciente.Messages.Patient;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.Patient
{
    public class PatientRepository: GenericDataService<PatientAggregate>
    {
        public PatientRepository(DbContext context) : base(context) { }

        public override async Task Update(PatientAggregate aggregate)
        {
            _dbSet.Attach(aggregate);

            _dbContext.Entry(aggregate).State = EntityState.Modified;
            if (aggregate.User != null) _dbContext.Entry(aggregate.User).State = EntityState.Modified;
            if (aggregate.Person != null)
            {
                aggregate.Person.PersonLocation.ToList().ForEach(pl => _dbContext.Entry(pl).State = EntityState.Modified);
                _dbContext.Entry(aggregate.Person).State = EntityState.Modified;
            }
            
            await _dbContext.SaveChangesAsync();
        }

        public override async Task Delete(Guid aggregateId)
        {
            var aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                if (aggregate.User != null) _dbContext.Entry(aggregate.User).State = EntityState.Deleted;
                if (aggregate.Person != null)
                {
                    aggregate.Person.PersonLocation.ToList().ForEach(pl => _dbContext.Entry(pl).State = EntityState.Deleted);
                    _dbContext.Entry(aggregate.Person).State = EntityState.Deleted;
                }
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
