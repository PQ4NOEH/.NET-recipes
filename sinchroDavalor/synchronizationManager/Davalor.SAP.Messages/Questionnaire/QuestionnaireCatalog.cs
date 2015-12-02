using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Questionnaire
{
    public partial class QuestionnaireCatalog
    {
       
        public QuestionnaireCatalog()
        {
            QuestionCatalog = new HashSet<QuestionCatalog>();
            QuestionnaireCatalogItem = new HashSet<QuestionnaireCatalogItem>();
        }

        public Guid Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string SubtitleKeyId { get; set; }

        [StringLength(100)]
        public string NameKeyId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<QuestionCatalog> QuestionCatalog { get; set; }

        public virtual ICollection<QuestionnaireCatalogItem> QuestionnaireCatalogItem { get; set; }
    }
}
