using Davalor.SAP.Messages.Currency;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Currency
{
    public class VisionLocalCurrencyContext : DbContext, ISynchroDbContext<CurrencyAggregate>
    {
        public VisionLocalCurrencyContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalCurrencyContext>(null);
        }
        public virtual DbSet<CurrencyAggregate> Country { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyAggregate>().ToTable("Currency");
            modelBuilder.Entity<CurrencyAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
