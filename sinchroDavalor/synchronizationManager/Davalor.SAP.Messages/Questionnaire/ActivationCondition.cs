using System;
using System.Collections.Generic;

namespace Davalor.SAP.Messages.Questionnaire
{
   
    public partial class ActivationCondition
    {
        public ActivationCondition()
        {
            ActivationCondition1 = new HashSet<ActivationCondition>();
            ActivationCondition11 = new HashSet<ActivationCondition>();
            ActivationConditionExpected = new HashSet<ActivationConditionExpected>();
        }

        public Guid Id { get; set; }

        public int Type { get; set; }

        public int Operator { get; set; }

        public Guid? ActivationCriteriaId { get; set; }

        public Guid? QuestionId { get; set; }

        public Guid? LeftConditionId { get; set; }

        public Guid? RightConditionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<ActivationCondition> ActivationCondition1 { get; set; }

        public virtual ActivationCondition ActivationCondition2 { get; set; }

        public virtual ICollection<ActivationCondition> ActivationCondition11 { get; set; }

        public virtual ActivationCondition ActivationCondition3 { get; set; }

        public virtual ActivationCriteria ActivationCriteria { get; set; }

        public virtual Question Question { get; set; }

        public virtual ICollection<ActivationConditionExpected> ActivationConditionExpected { get; set; }
    }
}
