using Davalor.SAP.Messages.Media;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Media
{
    public class VisionLocalMediaContext : DbContext, ISynchroDbContext<MediaAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalMediaContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalMediaContext>(null);
        }

        public virtual DbSet<MediaAggregate> Media { get; set; }
        public virtual DbSet<MediaDeviceGroup> MediaDeviceGroup { get; set; }
        public virtual DbSet<MediaMachine> MediaMachine { get; set; }
        public virtual DbSet<MediaServiceLevel> MediaServiceLevel { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaAggregate>().ToTable("Media");
            modelBuilder.Entity<MediaDeviceGroup>().ToTable("MediaDeviceGroup");
            modelBuilder.Entity<MediaMachine>().ToTable("MediaMachine");
            modelBuilder.Entity<MediaServiceLevel>().ToTable("MediaServiceLevel");

            modelBuilder.Entity<MediaAggregate>()
                 .Property(e => e.TimeStamp)
                 .HasPrecision(6);

            modelBuilder.Entity<MediaAggregate>()
                .HasMany(e => e.MediaDeviceGroup)
                .WithRequired(e => e.Media)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MediaAggregate>()
                .HasMany(e => e.MediaMachine)
                .WithRequired(e => e.Media)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MediaAggregate>()
                .HasMany(e => e.MediaServiceLevel)
                .WithRequired(e => e.Media)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MediaDeviceGroup>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<MediaMachine>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<MediaServiceLevel>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
