using System;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.Answer
{
    public partial class AnswerValues
    {
        public Guid Id { get; set; }

        public int Type { get; set; }

        [StringLength(2048)]
        public string ValueNumber { get; set; }

        [StringLength(2048)]
        public string ValueDecimal { get; set; }

        [StringLength(2048)]
        public string ValueBoolean { get; set; }

        public Guid AnswerId { get; set; }

        public Guid? ValueCatalogItemId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual AnswerAggregate Answer { get; set; }
    }
}
