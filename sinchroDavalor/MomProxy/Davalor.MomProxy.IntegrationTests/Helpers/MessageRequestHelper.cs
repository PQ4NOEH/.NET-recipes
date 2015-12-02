using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.MomProxy.IntegrationTests.Helpers
{
    public class MessageRequestHelper
    {
        static readonly IStringSerializer _serializer = new JsonSerializer();
        public static async Task DoRequest(BaseEvent @event, int port)
        {
            var url = string.Format("http://localhost:{0}/API/Message/NewMessage", port);
            var message = _serializer.Serialize<BaseEvent>(@event);
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers["Content-Type"] = "application/json; charset=utf-8";
                await client.UploadStringTaskAsync(new Uri(url), "POST", message);
            }
        }
    }
}
