using Davalor.PortalPaciente.Messages.Payable;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.Payable
{
    public class PayableRepository : GenericDataService<PayableAggregate>
    {
        public PayableRepository(DbContext context) : base(context) { }

        public override async Task Update(PayableAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            aggregate.PaymentTransaction.ToList().ForEach(t=> 
                {
                    _dbContext.Entry(t.Payment).State = EntityState.Modified;
                    _dbContext.Entry(t.Payment.PaymentMpos).State = EntityState.Modified;
                    t.PaymentTaxTransaction.ToList().ForEach(tt => _dbContext.Entry(tt).State = EntityState.Modified);
                    _dbContext.Entry(t).State = EntityState.Modified;
                });
            await _dbContext.SaveChangesAsync();
        }

        public override async Task Delete(Guid aggregateId)
        {
            PayableAggregate aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                aggregate.PaymentTransaction.ToList().ForEach(t =>
                {
                    t.PaymentTaxTransaction.ToList().ForEach(tt => _dbContext.Entry(tt).State = EntityState.Deleted);
                    _dbContext.Entry(t).State = EntityState.Deleted;
                });
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
