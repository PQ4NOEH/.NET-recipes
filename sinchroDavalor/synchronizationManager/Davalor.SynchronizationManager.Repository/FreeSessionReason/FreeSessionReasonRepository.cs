using Davalor.SAP.Messages.FreeSessionReason;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.FreeSessionReason
{
    public class FreeSessionReasonRepository : GenericDataService<FreeSessionReasonAggregate>
    {
        public FreeSessionReasonRepository(DbContext context) : base(context) { }
    }
}
