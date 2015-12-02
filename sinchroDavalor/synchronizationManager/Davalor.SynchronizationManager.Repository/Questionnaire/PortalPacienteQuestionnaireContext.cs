using Davalor.SAP.Messages.Questionnaire;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Questionnaire
{
    public class PortalPacienteQuestionnaireContext : DbContext, ISynchroDbContext<QuestionnaireAggregate>
    {
        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
        public PortalPacienteQuestionnaireContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteQuestionnaireContext>(null);
        }

        public virtual DbSet<ActivationCondition> ActivationCondition { get; set; }
        public virtual DbSet<ActivationConditionExpected> ActivationConditionExpected { get; set; }
        public virtual DbSet<ActivationCriteria> ActivationCriteria { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuestionCatalog> QuestionCatalog { get; set; }
        public virtual DbSet<QuestionnaireAggregate> Questionnaire { get; set; }
        public virtual DbSet<QuestionnaireCatalog> QuestionnaireCatalog { get; set; }
        public virtual DbSet<QuestionnaireCatalogItem> QuestionnaireCatalogItem { get; set; }
        public virtual DbSet<QuestionnaireNode> QuestionnaireNode { get; set; }
        public virtual DbSet<QuestionnaireNodes> QuestionnaireNodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivationCondition>().ToTable("ActivationCondition");
            modelBuilder.Entity<ActivationConditionExpected>().ToTable("ActivationConditionExpected");
            modelBuilder.Entity<ActivationCriteria>().ToTable("ActivationCriteria");
            modelBuilder.Entity<Question>().ToTable("Question");
            modelBuilder.Entity<QuestionCatalog>().ToTable("QuestionCatalog");
            modelBuilder.Entity<QuestionnaireAggregate>().ToTable("Questionnaire");
            modelBuilder.Entity<QuestionnaireCatalog>().ToTable("QuestionnaireCatalog");
            modelBuilder.Entity<QuestionnaireCatalogItem>().ToTable("QuestionnaireCatalogItem");
            modelBuilder.Entity<QuestionnaireNodes>().ToTable("QuestionnaireNodes");
            modelBuilder.Entity<QuestionnaireNode>().ToTable("QuestionnaireNode");

            modelBuilder.Entity<ActivationCondition>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ActivationCondition>()
                .HasMany(e => e.ActivationCondition1)
                .WithOptional(e => e.ActivationCondition2)
                .HasForeignKey(e => e.LeftConditionId);

            modelBuilder.Entity<ActivationCondition>()
                .HasMany(e => e.ActivationCondition11)
                .WithOptional(e => e.ActivationCondition3)
                .HasForeignKey(e => e.RightConditionId);

            modelBuilder.Entity<ActivationCondition>()
                .HasMany(e => e.ActivationConditionExpected)
                .WithRequired(e => e.ActivationCondition)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ActivationConditionExpected>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<ActivationCriteria>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<Question>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.QuestionCatalog)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<QuestionCatalog>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<QuestionnaireAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<QuestionnaireAggregate>()
                .HasMany(e => e.QuestionnaireNodes)
                .WithRequired(e => e.Questionnaire)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<QuestionnaireCatalog>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<QuestionnaireCatalog>()
                .HasMany(e => e.QuestionCatalog)
                .WithRequired(e => e.QuestionnaireCatalog)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<QuestionnaireCatalog>()
                .HasMany(e => e.QuestionnaireCatalogItem)
                .WithRequired(e => e.QuestionnaireCatalog)
                .HasForeignKey(e => e.CatalogId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<QuestionnaireCatalogItem>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<QuestionnaireCatalogItem>()
                .HasMany(e => e.ActivationConditionExpected)
                .WithOptional(e => e.QuestionnaireCatalogItem)
                .HasForeignKey(e => e.ExpectedValueCatalogItemId);

            modelBuilder.Entity<QuestionnaireNode>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<QuestionnaireNode>()
                .HasMany(e => e.QuestionnaireNode1)
                .WithOptional(e => e.QuestionnaireNode2)
                .HasForeignKey(e => e.ParentNodeId);

            modelBuilder.Entity<QuestionnaireNode>()
                .HasMany(e => e.QuestionnaireNodes)
                .WithRequired(e => e.QuestionnaireNode)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<QuestionnaireNodes>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
