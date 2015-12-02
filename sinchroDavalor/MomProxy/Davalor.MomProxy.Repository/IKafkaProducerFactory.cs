
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Kafka.Contracts;
using System;
namespace Davalor.MomProxy.Repository
{
    public interface IKafkaProducerFactory
    {
        IKafkaProducer CreateProducer(NotNullOrWhiteSpaceString topic);
    }
}
