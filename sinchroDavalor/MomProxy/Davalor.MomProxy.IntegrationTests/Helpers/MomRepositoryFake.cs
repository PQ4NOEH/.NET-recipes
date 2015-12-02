using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.MomProxy.IntegrationTests.Helpers
{
    public class MomRepositoryFake : IMomRepository
    {
        public readonly Dictionary<string, List<string>> ReceivedSendMessages = new Dictionary<string, List<string>>();


        public void SendMessages(NotNullOrWhiteSpaceString topic, NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> messages)
        {
            if (ReceivedSendMessages.ContainsKey(topic)) ReceivedSendMessages[topic].AddRange(messages.Value.Select(s => s.Value));
            else ReceivedSendMessages.Add(topic, messages.Value.Select(s => s.Value).ToList());
        }
    }
}
