using Davalor.SAP.Messages.Printer;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Printer
{
    public class VisionLocalPrinterContext : DbContext, ISynchroDbContext<PrinterAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalPrinterContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalPrinterContext>(null);
        }
        public virtual DbSet<PrinterAggregate> Gateway { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrinterAggregate>().ToTable("Printer");
            modelBuilder.Entity<PrinterAggregate>()
                 .Property(e => e.TimeStamp)
                 .HasPrecision(6);
        }
    }
}
