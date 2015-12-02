namespace Davalor.PortalPaciente.Messages.Answer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AnswerVisionLocalDbContext : DbContext
    {
        public AnswerVisionLocalDbContext()
            : base("name=AnswerVisionLocalDbContext")
        {
        }

        public virtual DbSet<AnswerAggregate> Answer { get; set; }
        public virtual DbSet<AnswerValues> AnswerValues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnswerAggregate>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);

            modelBuilder.Entity<AnswerAggregate>()
                .HasMany(e => e.AnswerValues)
                .WithRequired(e => e.Answer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnswerValues>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }
    }
}
