using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.VisionLocal.Messages.Session;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Session
{
    public class VisionLocalSessionContext : DbContext, ISynchroDbContext<SessionAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalSessionContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalSessionContext>(null);
        }

        public virtual DbSet<SessionDevice> SessionDevice { get; set; }
        public virtual DbSet<SessionAggregate> Session { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<Diagnosis>().ToTable("Diagnosis");
            modelBuilder.Entity<SessionDevice>().ToTable("SessionDevice");
            modelBuilder.Entity<SessionAggregate>().ToTable("Session");

            modelBuilder.Entity<Appointment>()
                 .Property(e => e.TimeStamp)
                 .HasPrecision(6);

            modelBuilder.Entity<Diagnosis>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.InitialTime)
                .HasPrecision(6);

            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.EndTime)
                .HasPrecision(6);

            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.SignedDate)
                .HasPrecision(6);

            modelBuilder.Entity<SessionAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<SessionAggregate>()
                .HasMany(e => e.Diagnosis)
                .WithRequired(e => e.Session)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SessionAggregate>()
                .HasMany(e => e.SessionDevice)
                .WithRequired(e => e.Session)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SessionDevice>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
