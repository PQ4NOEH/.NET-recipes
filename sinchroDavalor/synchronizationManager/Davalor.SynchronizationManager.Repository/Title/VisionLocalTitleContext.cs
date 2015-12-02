using Davalor.SAP.Messages.Title;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Title
{
    public class VisionLocalTitleContext : DbContext, ISynchroDbContext<TitleAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalTitleContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
        }
        public virtual DbSet<TitleAggregate> Title { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TitleAggregate>().ToTable("Title");

            modelBuilder.Entity<TitleAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
