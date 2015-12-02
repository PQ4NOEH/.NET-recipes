using Davalor.SAP.Messages.Tax;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Tax
{
    public class TaxRepository: GenericDataService<TaxAggregate>
    {
        public TaxRepository(DbContext context) : base(context) { }
    }
}
