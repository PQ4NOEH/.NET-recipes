using Davalor.SAP.Messages.Country;
using System.Data.Entity;
using System.Diagnostics;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class CountryRepository : GenericDataService<CountryAggregate>
    {
        public CountryRepository(DbContext context)
            : base(context) 
        {
        }

    }
}
