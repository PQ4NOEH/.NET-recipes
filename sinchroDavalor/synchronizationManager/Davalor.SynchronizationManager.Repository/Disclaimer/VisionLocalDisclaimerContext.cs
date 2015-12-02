using Davalor.PortalPaciente.Messages.Disclaimer;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class VisionLocalDisclaimerContext : DbContext, ISynchroDbContext<DisclaimerAggregate>
    {
        public VisionLocalDisclaimerContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalDisclaimerContext>(null);
        }
        public virtual DbSet<DisclaimerAggregate> Disclaimer { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisclaimerAggregate>().ToTable("Disclaimer");
            modelBuilder.Entity<DisclaimerAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
