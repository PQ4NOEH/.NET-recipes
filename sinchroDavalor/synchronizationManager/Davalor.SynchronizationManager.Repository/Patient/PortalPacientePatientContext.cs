using Davalor.PortalPaciente.Messages.Patient;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Patient
{
    public class PortalPacientePatientContext : DbContext, ISynchroDbContext<PatientAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacientePatientContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacientePatientContext>(null);
        }

        public virtual DbSet<PatientAggregate> Patient { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonLocation> PersonLocation { get; set; }
        //public virtual DbSet<Location> Location { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<PatientAggregate>().ToTable("Patient");
            modelBuilder.Entity<PersonLocation>().ToTable("PersonLocation");
            //modelBuilder.Entity<Location>().ToTable("Location");

            modelBuilder.Entity<PatientAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
            //modelBuilder.Entity<Location>()
            //   .Property(e => e.TimeStamp)
            //   .HasPrecision(6);

            modelBuilder.Entity<Person>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Patient)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Location>()
            //   .HasMany(e => e.PersonLocation)
            //   .WithRequired(e => e.Location)
            //   .WillCascadeOnDelete(true);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonLocation)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PersonLocation>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
