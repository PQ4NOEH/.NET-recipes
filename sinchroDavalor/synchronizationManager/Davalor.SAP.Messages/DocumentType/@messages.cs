


using Davalor.Base.Messaging.Contracts;
namespace Davalor.SAP.Messages.DocumentType
{
    public class RegisteredDocumentType : BaseEvent
    {
        public RegisteredDocumentType()
        {
            Topic = "DocumentType";
        }
    }
    public class ChangedDocumentType : BaseEvent
    {
        public ChangedDocumentType()
        {
            Topic = "DocumentType";
        }
    }
    public class UnregisteredDocumentType : BaseEvent
    {
        public UnregisteredDocumentType()
        {
            Topic = "DocumentType";
        }
    }
}
