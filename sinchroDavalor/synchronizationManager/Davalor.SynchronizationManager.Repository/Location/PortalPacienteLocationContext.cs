using Davalor.SAP.Messages.Location;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Media
{
    public class PortalPacienteLocationContext : DbContext, ISynchroDbContext<LocationAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacienteLocationContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteMediaContext>(null);
        }

        public virtual DbSet<LocationAggregate> Media { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationAggregate>().ToTable("Location");

            modelBuilder.Entity<LocationAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
