using Davalor.PortalPaciente.Messages.Patient;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Patient
{
    public class VisionLocalPatientContext : DbContext, ISynchroDbContext<PatientAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalPatientContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalPatientContext>(null);
        }

        public virtual DbSet<PatientAggregate> Patient { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonLocation> PersonLocation { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<PatientAggregate>().ToTable("Patient");
            modelBuilder.Entity<PersonLocation>().ToTable("PersonLocation");
            modelBuilder.Entity<User>().ToTable("User");

            modelBuilder.Entity<PatientAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<Person>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Patient)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonLocation)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PersonLocation>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.RecordDeletedDate)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.NewEmailRequest)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.ForgotPasswordRequest)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.LastChangePassword)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.LastLogon)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.RegistrationDate)
                .HasPrecision(6);

            modelBuilder.Entity<User>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
