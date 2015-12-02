using Davalor.SAP.Messages.Tax;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Tax
{
    public class VisionLocalTaxContext : DbContext, ISynchroDbContext<TaxAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalTaxContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
        }
        public virtual DbSet<TaxAggregate> Tax { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxAggregate>().ToTable("Tax");

            modelBuilder.Entity<TaxAggregate>()
                .Property(e => e.BeginPeriod)
                .HasPrecision(6);

            modelBuilder.Entity<TaxAggregate>()
                .Property(e => e.EndPeriod)
                .HasPrecision(6);

            modelBuilder.Entity<TaxAggregate>()
                .Property(e => e.Amount)
                .HasPrecision(11, 2);

            modelBuilder.Entity<TaxAggregate>()
                .Property(e => e.BaseAmount)
                .HasPrecision(11, 2);

            modelBuilder.Entity<TaxAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
