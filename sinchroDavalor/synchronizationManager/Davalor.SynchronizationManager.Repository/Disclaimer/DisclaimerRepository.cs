using Davalor.PortalPaciente.Messages.Disclaimer;
using System.Data.Entity;
using System.Diagnostics;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class DisclaimerRepository : GenericDataService<DisclaimerAggregate>
    {
        public DisclaimerRepository(DbContext context)
            : base(context) 
        {
        }

    }
}
