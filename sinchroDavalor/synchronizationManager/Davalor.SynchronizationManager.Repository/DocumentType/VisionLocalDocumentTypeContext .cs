using Davalor.SAP.Messages.DocumentType;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.DocumentType
{
    public class VisionLocalDocumentTypeContext : DbContext, ISynchroDbContext<DocumentTypeAggregate>
    {
        public VisionLocalDocumentTypeContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalDocumentTypeContext>(null);
        }
        public virtual DbSet<DocumentTypeAggregate> DocumentType { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentTypeAggregate>().ToTable("DocumentType");

            modelBuilder.Entity<DocumentTypeAggregate>()
               .Property(e => e.TimeStamp)
               .HasPrecision(6);
        }
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
