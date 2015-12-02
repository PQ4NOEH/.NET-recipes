using Davalor.SAP.Messages.Currency;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Currency
{
    public class CurrencyRepository : GenericDataService<CurrencyAggregate>
    {
        public CurrencyRepository(DbContext context) : base(context) {}
    }
}
