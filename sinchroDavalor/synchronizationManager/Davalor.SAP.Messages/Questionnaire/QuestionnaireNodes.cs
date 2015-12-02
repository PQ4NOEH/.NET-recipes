
using System;
namespace Davalor.SAP.Messages.Questionnaire
{

    public partial class QuestionnaireNodes
    {
        public Guid Id { get; set; }

        public Guid QuestionnaireId { get; set; }

        public Guid QuestionnaireNodeId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual QuestionnaireAggregate Questionnaire { get; set; }

        public virtual QuestionnaireNode QuestionnaireNode { get; set; }
    }
}
