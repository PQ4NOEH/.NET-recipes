using Davalor.PortalPaciente.Messages.Answer;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Davalor.SynchronizationManager.Repository.Answer
{
    public class AnswerRepository: GenericDataService<AnswerAggregate>
    {
        public AnswerRepository(DbContext context) : base(context) { }

        public override async Task Update(AnswerAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            aggregate.AnswerValues.ToList().ForEach(v => _dbContext.Entry(v).State = EntityState.Modified);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task Delete(Guid aggregateId)
        {
            var aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                aggregate.AnswerValues.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
