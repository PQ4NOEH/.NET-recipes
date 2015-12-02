using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Quota;
using NSubstitute;
using Xunit;
using Davalor.MomProxy.Services.UnitTests.ConfigurationFake;
using Davalor.MomProxy.Services;
using Davalor.Base.Library.Guards;
using Davalor.Toolkit.Extensions;

namespace Davalor.MomProxy.UnitTest.Forwarder
{
    public class MessageForwarderFactorySpec
    {
        [Fact]
        public void If_a_messageForwarder_for_a_given_topic_does_not_exist_creates_it()
        {
            NotNullable<IMomRepository> momRepository = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>()); 
            NotNullable<IServiceEvents> mediator = new ServiceEvents();
            NotNullable<IQuotaFactory> quotaFactory = new QuotaFactory(new HostConfiguration());
            var sut = new MessageForwarderFactory(momRepository, mediator, quotaFactory);
            Assert.NotNull(sut.CreateForwarder(StringExtension.RandomString()));
        }
        
        [Fact]
        public void If_a_messageForwarder_for_a_given_topic_already_exist_returns_that_instance()
        {
            NotNullable<IMomRepository> momRepository = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            NotNullable<IServiceEvents> mediator = new ServiceEvents();
            NotNullable<IQuotaFactory> quotaFactory = new QuotaFactory(new HostConfiguration());
            var sut = new MessageForwarderFactory(momRepository, mediator, quotaFactory);
            var topic = StringExtension.RandomString();
            var forwarder = sut.CreateForwarder(topic);
            Assert.Same(sut.CreateForwarder(topic), forwarder);
        }
    }
}
