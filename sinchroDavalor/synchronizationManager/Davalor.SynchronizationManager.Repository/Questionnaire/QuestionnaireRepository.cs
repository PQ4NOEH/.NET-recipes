using Davalor.SAP.Messages.Questionnaire;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.Questionnaire
{
    public class QuestionnaireRepository : GenericDataService<QuestionnaireAggregate>
    {
        public QuestionnaireRepository(DbContext context) : base(context) { }
    }
}
