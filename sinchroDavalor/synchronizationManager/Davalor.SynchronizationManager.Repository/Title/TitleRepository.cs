using Davalor.SAP.Messages.Title;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Title
{
    public class TitleRepository: GenericDataService<TitleAggregate>
    {
        public TitleRepository(DbContext context) : base(context) { }
    }
}
