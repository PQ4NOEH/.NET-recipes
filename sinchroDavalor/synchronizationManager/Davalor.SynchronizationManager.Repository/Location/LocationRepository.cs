using Davalor.SAP.Messages.Location;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace Davalor.SynchronizationManager.Repository.Media
{
    public class LocationRepository : GenericDataService<LocationAggregate>
    {
        public LocationRepository(DbContext context) : base(context) { }
    }

}
