using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Davalor.PortalPaciente.Messages.Answer;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;

namespace Davalor.SynchronizationManager.Repository.Answer
{


    public partial class PortalPacienteAnswerContext : DbContext, ISynchroDbContext<AnswerAggregate>
    {
        public PortalPacienteAnswerContext(IHostConfiguration hostConfiguration)
            : base("PortalPaciente")
        {
            Database.SetInitializer<PortalPacienteAnswerContext>(null);
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
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<AnswerValues>()
                .Property(e => e.TimeStamp)
                .HasPrecision(6);
        }

        public ESynchroSystem SynchroSystem
        {
            get { return ESynchroSystem.PortalPaciente; }
        }
    }
}
