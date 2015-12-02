using Davalor.PortalPaciente.Messages.DisclaimerSignature;
using System.Data.Entity;
using System.Diagnostics;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class DisclaimerSignatureRepository : GenericDataService<DisclaimerSignatureAggregate>
    {
        public DisclaimerSignatureRepository(DbContext context)
            : base(context) 
        {
        }

    }
}
