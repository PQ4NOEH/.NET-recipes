using Davalor.SAP.Messages.PaymentGateway;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.PaymentGateway
{
    public class PaymentGatewayRepository: GenericDataService<GatewayAggregate>
    {
        public PaymentGatewayRepository(DbContext context) : base(context) { }

        public override async Task Update(GatewayAggregate aggregate)
        {
            _dbSet.Attach(aggregate);

            _dbContext.Entry(aggregate).State = EntityState.Modified;
            aggregate.GatewayByCountry.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Modified);

            await _dbContext.SaveChangesAsync();
        }

        public override async Task Delete(Guid aggregateId)
        {
            var aggregate = await _dbSet.FindAsync(aggregateId);
            if (aggregate != null)
            {
                aggregate.GatewayByCountry.ToList().ForEach(m => _dbContext.Entry(m).State = EntityState.Deleted);
                _dbContext.Entry(aggregate).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
