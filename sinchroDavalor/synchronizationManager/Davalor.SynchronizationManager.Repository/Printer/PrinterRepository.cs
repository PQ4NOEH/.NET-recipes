using Davalor.SAP.Messages.Printer;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Printer
{
    public class PrinterRepository : GenericDataService<PrinterAggregate>
    {
        public PrinterRepository(DbContext context) : base(context) { }
    }
}
