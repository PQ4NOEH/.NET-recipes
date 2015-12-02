using Davalor.SAP.Messages.Device;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Device
{
    public class VisionLocalDeviceContext : DbContext, ISynchroDbContext<DeviceAggregate>
    {
        public VisionLocalDeviceContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalDeviceContext>(null);
        }

        public virtual DbSet<DeviceAggregate> Device { get; set; }
        public virtual DbSet<DeviceGroup> DeviceGroup { get; set; }
        public virtual DbSet<DeviceType> DeviceType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceAggregate>().ToTable("Device");
            modelBuilder.Entity<DeviceGroup>().ToTable("DeviceGroup");
            modelBuilder.Entity<DeviceType>().ToTable("DeviceType");

            modelBuilder.Entity<DeviceAggregate>()
                   .Property(e => e.TimeStamp)
                   .HasPrecision(6);

            modelBuilder.Entity<DeviceGroup>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<DeviceGroup>()
                .HasMany(e => e.Device)
                .WithRequired(e => e.DeviceGroup)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DeviceType>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<DeviceType>()
                .HasMany(e => e.DeviceGroup)
                .WithRequired(e => e.DeviceType)
                .WillCascadeOnDelete(true);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
