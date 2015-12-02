using Davalor.SAP.Messages.Partner;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Partner
{
    public class VisionLocalPartnerContext : DbContext, ISynchroDbContext<PartnerAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalPartnerContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalPartnerContext>(null);
        }

        
        public virtual DbSet<PartnerAggregate> Partner { get; set; }
        public virtual DbSet<PartnerChain> PartnerChain { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<PartnerAggregate>().ToTable("Partner");
            modelBuilder.Entity<PartnerChain>().ToTable("PartnerChain");

            modelBuilder.Entity<PartnerAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<PartnerChain>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
