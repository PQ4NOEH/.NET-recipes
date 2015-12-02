using Davalor.SAP.Messages.Partner;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.Partner
{
    public class PartnerRepository : GenericDataService<PartnerAggregate>
    {
        public PartnerRepository(DbContext context) : base(context) { }

        public override async Task Update(PartnerAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            if (aggregate.PartnerChain != null) _dbContext.Entry(aggregate.PartnerChain).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
}
