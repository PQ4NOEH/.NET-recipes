using System.Data.Entity;
using Davalor.PortalPaciente.Messages.Answer;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;

namespace Davalor.SynchronizationManager.Repository.Answer
{


    public partial class VisionLocalAnswerContext : DbContext, ISynchroDbContext<AnswerAggregate>
    {
        public VisionLocalAnswerContext(IHostConfiguration hostConfiguration)
            : base("VisionLocal")
        {
            Database.SetInitializer<VisionLocalAnswerContext>(null);
        }

        public virtual DbSet<AnswerAggregate> Answer { get; set; }
        public virtual DbSet<AnswerValues> AnswerValues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnswerAggregate>().ToTable("Answer");
            modelBuilder.Entity<AnswerValues>().ToTable("AnswerValues");

            modelBuilder.Entity<AnswerAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<AnswerAggregate>()
                .HasMany(e => e.AnswerValues)
                .WithRequired(e => e.Answer)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<AnswerValues>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.VisionLocal; }
        }
    }
}
