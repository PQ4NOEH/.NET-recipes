using Davalor.SAP.Messages.Service;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Service
{
    public class PortalPacienteServiceContext : DbContext, ISynchroDbContext<ServiceAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacienteServiceContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteServiceContext>(null);
        }
        public virtual DbSet<ServiceAggregate> Service { get; set; }
        public virtual DbSet<ServiceLevel> ServiceLevel { get; set; }
        public virtual DbSet<ServicePrice> ServicePrice { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }
        public virtual DbSet<ServiceAggregate> Region { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceAggregate>().ToTable("Service");
            modelBuilder.Entity<ServiceLevel>().ToTable("ServiceLevel");
            modelBuilder.Entity<ServicePrice>().ToTable("ServicePrice");
            modelBuilder.Entity<ServiceType>().ToTable("ServiceType");

            modelBuilder.Entity<ServiceAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ServiceAggregate>()
                .HasMany(e => e.ServiceLevel)
                .WithRequired(e => e.Service)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ServiceLevel>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ServiceLevel>()
                .HasMany(e => e.ServicePrice)
                .WithRequired(e => e.ServiceLevel)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ServicePrice>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ServiceType>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ServiceType>()
                .HasMany(e => e.Service)
                .WithRequired(e => e.ServiceType)
                .WillCascadeOnDelete(true);
        }
    }
}
