using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.MomProxy.Repository;
using Davalor.MomProxy.Services;
using System.Web.Http;

namespace Davalor.MomProxy.ConsoleHost
{
    [RoutePrefix("API/Message")]
    public class MessageController : ApiController
    {
        readonly IncommingMessageService _service;
        public MessageController()
        {
            _service = new IncommingMessageService(new IncommingMessageRepository(), ServiceEvents.Instance.Value);
        }

        [HttpPost]
        [Route("NewMessage")]
        public void NewMessage(BaseEvent incommingMessage)
        {
            if (incommingMessage.IsValid())
            {
                var message = new JsonSerializer().Serialize<BaseEvent>(incommingMessage);
                _service.NewMessage(message);
            }
            else
            {
                ServiceEvents.Instance.Value.ReceivedInvalidMessage(new BaseEvent
                {
                    Topic = incommingMessage.Topic,
                    InvalidReason = incommingMessage.InvalidReason.Value
                });
            }
        }
    }
}
