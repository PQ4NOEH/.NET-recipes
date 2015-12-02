using Davalor.SAP.Messages.Region;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Region
{
    public class RegionRepository : GenericDataService<RegionAggregate>
    {
        public RegionRepository(DbContext context) : base(context) { }
    }
}
