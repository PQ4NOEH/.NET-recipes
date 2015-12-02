using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.VisionLocal.Messages.Session;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Session
{
    public class PortalPacienteSessionContext : DbContext, ISynchroDbContext<SessionAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacienteSessionContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteSessionContext>(null);
        }

        public virtual DbSet<SessionAggregate> Session { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionAggregate>().ToTable("Session");
            modelBuilder.Entity<Appointment>().ToTable("Appoinment");
            modelBuilder.Entity<Diagnosis>().ToTable("Diagnosis");

            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.EndTime)
                .HasPrecision(6);
            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.InitialTime)
                .HasPrecision(6);
            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.SignedDate)
                .HasPrecision(6);
            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
            modelBuilder.Entity<Appointment>()
               .Property(e => e.FinalTime)
               .HasPrecision(6);
            modelBuilder.Entity<Appointment>()
               .Property(e => e.InitialTime)
               .HasPrecision(6);
            modelBuilder.Entity<Diagnosis>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
