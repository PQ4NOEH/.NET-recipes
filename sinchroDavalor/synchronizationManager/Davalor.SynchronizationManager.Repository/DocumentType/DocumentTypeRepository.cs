using Davalor.SAP.Messages.DocumentType;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.DocumentType
{
    public class DocumentTypeRepository : GenericDataService<DocumentTypeAggregate>
    {
        public DocumentTypeRepository(DbContext context) : base(context) { }
    }
}
