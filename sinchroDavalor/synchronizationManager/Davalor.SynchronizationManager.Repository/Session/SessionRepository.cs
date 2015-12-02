using Davalor.VisionLocal.Messages.Session;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Session
{
    public class SessionRepository : GenericDataService<SessionAggregate>
    {
        public SessionRepository(DbContext context) : base(context) { }
    }
}
