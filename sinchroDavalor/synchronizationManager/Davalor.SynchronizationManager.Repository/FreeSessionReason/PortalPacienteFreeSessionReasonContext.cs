using Davalor.SAP.Messages.FreeSessionReason;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.FreeSessionReason
{
    public class PortalPacienteFreeSessionReasonContext : DbContext, ISynchroDbContext<FreeSessionReasonAggregate>
    {
        public PortalPacienteFreeSessionReasonContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteFreeSessionReasonContext>(null);
        }
        public virtual DbSet<FreeSessionReasonAggregate> DocumentType { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FreeSessionReasonAggregate>().ToTable("FreeSessionReason");
            modelBuilder.Entity<FreeSessionReasonAggregate>()
               .Property(e => e.TimeStamp)
               .HasPrecision(6);
        }
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
    }
}
