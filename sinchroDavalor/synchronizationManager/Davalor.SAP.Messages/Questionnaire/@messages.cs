using Davalor.Base.Messaging.Contracts;

namespace Davalor.SAP.Messages.Questionnaire
{
    public class RegisteredQuestionnaire : BaseEvent 
    {
        public RegisteredQuestionnaire()
        {
            Topic = "Questionnaire";
        }
    }
    public class ChangedQuestionnaire : BaseEvent  
    {
        public ChangedQuestionnaire()
        {
            Topic = "Questionnaire";
        }
    }
    public class UnregisteredQuestionnaire : BaseEvent 
    {
        public UnregisteredQuestionnaire()
        {
            Topic = "Questionnaire";
        }
    }
}
