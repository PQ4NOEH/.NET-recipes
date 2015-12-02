using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class QuestionCatalog
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Guid QuestionnaireCatalogId { get; set; }

        public Guid? ActivationCriteriaId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ActivationCriteria ActivationCriteria { get; set; }

        public virtual Question Question { get; set; }

        public virtual QuestionnaireCatalog QuestionnaireCatalog { get; set; }
    }
}
