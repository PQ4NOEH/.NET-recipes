using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class QuestionnaireNode
    {
        public QuestionnaireNode()
        {
            QuestionnaireNode1 = new HashSet<QuestionnaireNode>();
            QuestionnaireNodes = new HashSet<QuestionnaireNodes>();
        }

        public Guid Id { get; set; }

        [StringLength(200)]
        public string RenderTemplate { get; set; }

        [StringLength(200)]
        public string RenderClass { get; set; }

        [StringLength(100)]
        public string TitleKeyId { get; set; }

        public int Sequence { get; set; }

        public int Type { get; set; }

        public bool? Collapsed { get; set; }

        public bool Enabled { get; set; }

        public bool Mandatory { get; set; }

        [StringLength(200)]
        public string Layout { get; set; }

        public int? Level { get; set; }

        public Guid? ActivationCriteriaId { get; set; }

        public Guid? ParentNodeId { get; set; }

        public Guid? QuestionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ActivationCriteria ActivationCriteria { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<QuestionnaireNode> QuestionnaireNode1 { get; set; }

        public virtual QuestionnaireNode QuestionnaireNode2 { get; set; }
        public virtual ICollection<QuestionnaireNodes> QuestionnaireNodes { get; set; }
    }
}
