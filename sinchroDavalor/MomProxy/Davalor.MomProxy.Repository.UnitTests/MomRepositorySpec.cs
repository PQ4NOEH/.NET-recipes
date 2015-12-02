using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Kafka.Contracts;
using Davalor.Toolkit.Extensions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.Repository.UnitTests
{
    public class MomRepositorySpec
    {
        [Fact]
        public void If_receives_more_than_one_message_it_send_them_in_batch()
        {
            var producerFactory = new KafkaProducerFactoryFake();
            var sut = new MomRepository(producerFactory);
            var messages = StringExtension.RandomStrings(14, 23).Select(s=> new NotNullOrWhiteSpaceString(s)).ToList();
            var topic = StringExtension.RandomString();
            sut.SendMessages(topic, messages);
            producerFactory.Producer.Received(1).Produce(Arg.Any<NotNullable<IEnumerable<string>>>(), topic);
            producerFactory.Producer.DidNotReceive().Produce(Arg.Any<NotNullable<string>>(), topic);
        }
        [Fact]
        public void If_receives_just_one_message_it_send_it()
        {
            var producerFactory = new KafkaProducerFactoryFake();
            var sut = new MomRepository(producerFactory);
            var messages = StringExtension.RandomStrings(1, 23).Select(s => new NotNullOrWhiteSpaceString(s)).ToList();
            var topic = StringExtension.RandomString();
            sut.SendMessages(topic, messages);
            producerFactory.Producer.DidNotReceive().Produce(Arg.Any<NotNullable<IEnumerable<string>>>(), topic);
            producerFactory.Producer.Received(1).Produce(Arg.Any<NotNullable<string>>(), topic);
        }

        class KafkaProducerFactoryFake : IKafkaProducerFactory
        {
            public IKafkaProducer Producer;
           
            public IKafkaProducer CreateProducer(NotNullOrWhiteSpaceString topic)
            {
                if (Producer == null) Producer = Substitute.For<IKafkaProducer>();
                return Producer;
            }
        }
    }
}
