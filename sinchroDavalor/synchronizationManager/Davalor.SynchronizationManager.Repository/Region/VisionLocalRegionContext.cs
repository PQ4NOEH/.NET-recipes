using Davalor.SAP.Messages.Region;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Region
{
    public class VisionLocalRegionContext : DbContext, ISynchroDbContext<RegionAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalRegionContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalRegionContext>(null);
        }

        public virtual DbSet<RegionAggregate> Region { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegionAggregate>().ToTable("Region");
            modelBuilder.Entity<RegionAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
