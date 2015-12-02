using Davalor.PortalPaciente.Messages.Payable;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Payable
{
    public class PortalPacientePayableContext : DbContext, ISynchroDbContext<PayableAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacientePayableContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacientePayableContext>(null);
        }

        public virtual DbSet<PayableAggregate> Payable { get; set; }
        public virtual DbSet<PaymentEntity> Payment { get; set; }
        public virtual DbSet<PaymentMpos> PaymentMpos { get; set; }
        public virtual DbSet<PaymentTaxTransaction> PaymentTaxTransaction { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransaction { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PayableAggregate>().ToTable("Payable");
            modelBuilder.Entity<PaymentEntity>().ToTable("Payment");
            modelBuilder.Entity<PaymentMpos>().ToTable("PaymentMpos");
            modelBuilder.Entity<PaymentTaxTransaction>().ToTable("PaymentTaxTransaction");
            modelBuilder.Entity<PaymentTransaction>().ToTable("PaymentTransaction");

            modelBuilder.Entity<PayableAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PayableAggregate>()
                .HasMany(e => e.PaymentTransaction)
                .WithRequired(e => e.Payable)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PaymentEntity>()
                .Property(e => e.Amount)
                .HasPrecision(11, 2);

            modelBuilder.Entity<PaymentEntity>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PaymentEntity>()
                .HasOptional(e => e.PaymentMpos)
                .WithRequired(e => e.Payment);

            modelBuilder.Entity<PaymentEntity>()
                .HasMany(e => e.PaymentTransaction)
                .WithRequired(e => e.Payment)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PaymentMpos>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PaymentTaxTransaction>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.Amount)
                .HasPrecision(11, 2);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PaymentTransaction>()
                .HasMany(e => e.PaymentTaxTransaction)
                .WithRequired(e => e.PaymentTransaction)
                .WillCascadeOnDelete(true);
        }
    }
}
