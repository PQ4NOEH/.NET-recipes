
using Davalor.Base.Messaging.Kafka;
using Davalor.Toolkit.Extensions;
using Xunit;

namespace Davalor.MomProxy.Repository.UnitTests
{
    public class KafkaProducerFactorySpec
    {
        [Fact]
        public void If_a_producer_for_a_given_topic_does_not_exist_creates_it()
        {
            var sut = new KafkaProducerFactory(new KafkaConfiguration());
            Assert.NotNull(sut.CreateProducer(StringExtension.RandomString()));
        }

        [Fact]
        public void If_a_producer_for_a_given_topic_already_exist_returns_that_instance()
        {
            var sut = new KafkaProducerFactory(new KafkaConfiguration());
            var topic = StringExtension.RandomString();
            var producer = sut.CreateProducer(topic);
            Assert.Same(producer, sut.CreateProducer(topic));
        }
    }
}
