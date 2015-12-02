
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class QuestionnaireAggregate : ISynchroAggregateRoot
    {
        public QuestionnaireAggregate()
        {
            QuestionnaireNodes = new HashSet<QuestionnaireNodes>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameKeyId { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        public int Application { get; set; }

        public int Version { get; set; }

        public bool Active { get; set; }

        public bool Locked { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<QuestionnaireNodes> QuestionnaireNodes { get; set; }
    }
}
