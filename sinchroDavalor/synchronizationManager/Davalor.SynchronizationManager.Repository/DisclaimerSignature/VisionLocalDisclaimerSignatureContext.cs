using Davalor.PortalPaciente.Messages.DisclaimerSignature;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Country
{
    public class VisionLocalDisclaimerSignatureContext : DbContext, ISynchroDbContext<DisclaimerSignatureAggregate>
    {
        public VisionLocalDisclaimerSignatureContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalDisclaimerSignatureContext>(null);
        }
        public virtual DbSet<DisclaimerSignatureAggregate> DisclaimerSignature { get; set; }
        public virtual DbSet<Signature> Signature { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisclaimerSignatureAggregate>().ToTable("DavalorDisclaimer");
            modelBuilder.Entity<Signature>().ToTable("Signature");

            //modelBuilder.Entity<DisclaimerSignatureAggregate>()
            //    .HasRequired(e => e.Signature)
            //    .WithRequiredDependent(e => e.DisclaimerSignature)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<DisclaimerSignatureAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
            modelBuilder.Entity<DisclaimerSignatureAggregate>()
                .Property(e => e.SignedDate)
                .HasPrecision(6);

            modelBuilder.Entity<Signature>()
               .Property(e => e.TimeStamp)
               .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
