using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Kafka;
using Davalor.Base.Messaging.Kafka.Contracts;
using System.Collections.Generic;

namespace Davalor.MomProxy.Repository
{
    public class KafkaProducerFactory : IKafkaProducerFactory
    {
        readonly NotNullable<IKafkaConfiguration> _configuration;
        readonly Dictionary<string, IKafkaProducer> _producers = new Dictionary<string, IKafkaProducer>();
        readonly NotNullable<IBinarySerializer> _serializer = new BinaryJsonSerializer();
        public KafkaProducerFactory(NotNullable<IKafkaConfiguration> configuration)
        {
            _configuration = configuration;
        }

        public IKafkaProducer CreateProducer(NotNullOrWhiteSpaceString topic)
        {
            if(!_producers.ContainsKey(topic))
            {
                _producers.Add(topic, new KafkaProducer(_configuration, _serializer));
            }
            return _producers[topic];
        }
    }
}
