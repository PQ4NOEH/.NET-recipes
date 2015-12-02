using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class ActivationCriteria
    {
        public ActivationCriteria()
        {
            ActivationCondition = new HashSet<ActivationCondition>();
            QuestionCatalog = new HashSet<QuestionCatalog>();
            QuestionnaireNode = new HashSet<QuestionnaireNode>();
        }

        public Guid Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Type { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<ActivationCondition> ActivationCondition { get; set; }

        public virtual ICollection<QuestionCatalog> QuestionCatalog { get; set; }

        public virtual ICollection<QuestionnaireNode> QuestionnaireNode { get; set; }
    }
}
