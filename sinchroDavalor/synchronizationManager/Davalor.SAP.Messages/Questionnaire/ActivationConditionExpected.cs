using System;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class ActivationConditionExpected
    {
        public Guid Id { get; set; }

        public int Type { get; set; }

        public int? ExpectedValueInteger { get; set; }

        public decimal? ExpectedValueDecimal { get; set; }

        public int? ExpectedValueBoolean { get; set; }

        public Guid ActivationConditionId { get; set; }

        public Guid? ExpectedValueCatalogItemId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ActivationCondition ActivationCondition { get; set; }

        public virtual QuestionnaireCatalogItem QuestionnaireCatalogItem { get; set; }
    }
}
