using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.VisionLocal.Messages.AuditLogon;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.AuditLogon
{
    public class PortalPacienteAuditLogonContext : DbContext, ISynchroDbContext<AuditLogonAggregate>
    {
        public PortalPacienteAuditLogonContext(IHostConfiguration config)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteAuditLogonContext>(null);
        }

        public virtual DbSet<AuditLogonAggregate> AuditLogon { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLogonAggregate>().ToTable("AuditLogon");

            modelBuilder.Entity<AuditLogonAggregate>()
                .Property(e => e.AccessDate)
                .HasPrecision(6);
            modelBuilder.Entity<AuditLogonAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
    }
}
