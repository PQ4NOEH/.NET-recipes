using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.Toolkit.Extensions;
using NSubstitute;
using Xunit;

namespace Davalor.MomProxy.Services.UnitTests
{
    public class IncommingMessageServiceSpec
    {
        [Fact]
        public void When_receives_a_message_persist_it()
        {
            var repository = Substitute.For<IIncommingMessageRepository>();
            var serviceEvents = Substitute.For<IServiceEvents>();
            var sut = new IncommingMessageService(repository, serviceEvents);
            var message = new NotNullOrWhiteSpaceString(StringExtension.RandomString());
            sut.NewMessage(message);
            repository.Received(1).Save(message);
        }

        [Fact]
        public void When_receives_a_message_notifies_it_has_persisted_it()
        {
            var repository = Substitute.For<IIncommingMessageRepository>();
            var serviceEvents = Substitute.For<IServiceEvents>();
            var sut = new IncommingMessageService(repository, serviceEvents);
            var message = new NotNullOrWhiteSpaceString(StringExtension.RandomString());
            sut.NewMessage(message);
            serviceEvents.Received(1).SavedIncommingMessage(message);
        }
    }
}
