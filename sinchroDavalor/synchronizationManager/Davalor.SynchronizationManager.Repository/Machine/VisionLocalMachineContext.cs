using Davalor.SAP.Messages.Machine;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Machine
{
    public class VisionLocalMachineContext : DbContext, ISynchroDbContext<MachineAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
        public VisionLocalMachineContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalMachineContext>(null);
        }

        public virtual DbSet<MachineAggregate> Machine { get; set; }
        public virtual DbSet<MachineGroup> MachineGroup { get; set; }
        public virtual DbSet<MachinePrinter> MachinePrinter { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MachineAggregate>().ToTable("Machine");
            modelBuilder.Entity<MachineGroup>().ToTable("MachineGroup");
            modelBuilder.Entity<MachinePrinter>().ToTable("MachinePrinter");

            modelBuilder.Entity<MachineAggregate>()
                 .Property(e => e.TimeStamp)
                 .HasPrecision(6);

            modelBuilder.Entity<MachineAggregate>()
                .HasMany(e => e.MachinePrinter)
                .WithRequired(e => e.Machine)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MachineGroup>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<MachineGroup>()
                .HasMany(e => e.Machine)
                .WithRequired(e => e.MachineGroup)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MachinePrinter>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
        
    }
}
