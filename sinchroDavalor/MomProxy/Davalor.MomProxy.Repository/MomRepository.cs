using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Davalor.MomProxy.Repository
{
    public class MomRepository : IMomRepository
    {
        readonly IKafkaProducerFactory _producerFactory;
        public MomRepository(NotNullable<IKafkaProducerFactory> producerFactory)
        {
            _producerFactory = producerFactory.Value;
        }
        public void SendMessage(NotNullOrWhiteSpaceString topic, NotNullOrWhiteSpaceString message)
        {
            _producerFactory.CreateProducer(topic).Produce<string>(message.Value, topic);
        }
        public void SendMessages(NotNullOrWhiteSpaceString topic, NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> messages)
        {
            if (messages.Value.Count() == 1) SendMessage(topic, messages.Value.First());
            else
            {
                _producerFactory.CreateProducer(topic).Produce<string>(messages.Value.Select(m => m.Value).ToList(), topic);
            }
        }
    }
}
