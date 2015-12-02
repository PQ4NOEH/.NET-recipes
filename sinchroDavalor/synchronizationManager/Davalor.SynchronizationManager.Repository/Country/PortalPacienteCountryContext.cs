using Davalor.SAP.Messages.Country;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class PortalPacienteCountryContext : DbContext, ISynchroDbContext<CountryAggregate>
    {
        public PortalPacienteCountryContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteCountryContext>(null);
        }
        public virtual DbSet<CountryAggregate> Country { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryAggregate>().ToTable("Country");
            modelBuilder.Entity<CountryAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
    }
}
