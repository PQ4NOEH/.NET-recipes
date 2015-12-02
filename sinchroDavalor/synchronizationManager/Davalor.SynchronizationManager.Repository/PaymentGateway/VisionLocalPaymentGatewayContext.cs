using Davalor.SAP.Messages.PaymentGateway;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.PaymentGateway
{
    public class VisionLocalPaymentGatewayContext : DbContext, ISynchroDbContext<GatewayAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalPaymentGatewayContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalPaymentGatewayContext>(null);
        }
        public virtual DbSet<GatewayAggregate> Gateway { get; set; }
        public virtual DbSet<GatewayByCountry> GatewayByCountry { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GatewayAggregate>().ToTable("Gateway");
            modelBuilder.Entity<GatewayByCountry>().ToTable("GatewayByCountry");

            modelBuilder.Entity<GatewayAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<GatewayAggregate>()
                .HasMany(e => e.GatewayByCountry)
                .WithRequired(e => e.Gateway)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<GatewayByCountry>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
