using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class QuestionnaireCatalogItem
    {

        public QuestionnaireCatalogItem()
        {
            ActivationConditionExpected = new HashSet<ActivationConditionExpected>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TitleKeyId { get; set; }

        public byte[] Image { get; set; }

        public int? Type { get; set; }

        public int CatalogItemCode { get; set; }

        public int? Sequence { get; set; }

        public bool Enabled { get; set; }

        public Guid CatalogId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
        public virtual ICollection<ActivationConditionExpected> ActivationConditionExpected { get; set; }

        public virtual QuestionnaireCatalog QuestionnaireCatalog { get; set; }
    }
}
