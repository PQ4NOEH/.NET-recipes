using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.Toolkit.Extensions;
using NSubstitute;
using System;
using Xunit;

namespace Davalor.MomProxy.Services.UnitTests
{
    public class MessageCleanerServiceSpec
    {
        [Fact]
        public void When_a_message_is_sent_it_removes_the_message_from_the_repository()
        {
            var repository = new NotNullable<IIncommingMessageRepository>( Substitute.For<IIncommingMessageRepository>());
            var serviceEvents = new ServiceEvents();
            var sut = new MessageCleanerService(serviceEvents, repository);
            var message = new NotNullOrWhiteSpaceString(StringExtension.RandomString());
            serviceEvents.SentIncommingMessage(message);
            repository.Value.Received(1).Delete(message);
        }

        [Fact]
        public void When_a_message_is_removed_emits_serviceEvent_DeletedIncommingMessage()
        {
            var repository = new NotNullable<IIncommingMessageRepository>(Substitute.For<IIncommingMessageRepository>());
            var serviceEvents = new ServiceEvents();
            var sut = new MessageCleanerService(serviceEvents, repository);
            var message = new NotNullOrWhiteSpaceString(StringExtension.RandomString());
            bool eventEmitted = false;
            serviceEvents.DeletedIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
            {
                Assert.Equal(message.Value, s.Value);
                eventEmitted = true;
            }));
            serviceEvents.SentIncommingMessage(message);
            Assert.True(eventEmitted);
        }
    }
}
