using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Services;
using Davalor.Toolkit.Extensions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Davalor.MomProxy.UnitTest.Forwarder
{
    public class MessageForwarderSpec
    {
        [Fact]
        public void If_wont_send_incommingMessages_until_we_call_StartForwarding()
        {
            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var sut = new MessageForwarder(StringExtension.RandomString(), repo, new ServiceEvents(), new TransparentQuota());
            (3).Times(new Action(() => sut.AddMessage(StringExtension.RandomString())));
            Thread.Sleep(100);
            repo.Value.DidNotReceive().SendMessages(Arg.Any<NotNullOrWhiteSpaceString>(), Arg.Any<NotNullable<IEnumerable<NotNullOrWhiteSpaceString>>>());
        }
        [Fact]
        public void It_sends_added_Messages_on_a_background_thread()
        {
            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var sut = new MessageForwarder(StringExtension.RandomString(), repo, new ServiceEvents(), new TransparentQuota());
            (3).Times(new Action(() => sut.AddMessage(StringExtension.RandomString())));
            sut.StartForwarding();
            Thread.Sleep(200);
            repo.Value.Received().SendMessages(Arg.Any<NotNullOrWhiteSpaceString>(), Arg.Any<NotNullable<IEnumerable<NotNullOrWhiteSpaceString>>>());
        }
        [Fact]
        public void If_we_call_StopForwarding_it_does_not_send_any_incomming_message()
        {
            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var sut = new MessageForwarder(StringExtension.RandomString(), repo, new ServiceEvents(), new TransparentQuota());
           
            sut.StartForwarding();
            sut.StopForwarding();
            (3).Times(new Action(() => sut.AddMessage(StringExtension.RandomString())));
            Thread.Sleep(200);

            repo.Value.DidNotReceive().SendMessages(Arg.Any<NotNullOrWhiteSpaceString>(), Arg.Any<NotNullable<IEnumerable<NotNullOrWhiteSpaceString>>>());
        }
        [Fact]
        public void Can_be_start_to_send_messages_again_after_being_Stoped()
        {
            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var sut = new MessageForwarder(StringExtension.RandomString(), repo, new ServiceEvents(), new TransparentQuota());

            sut.StartForwarding();
            sut.StopForwarding();
            sut.StartForwarding();
            (3).Times(new Action(() => sut.AddMessage(StringExtension.RandomString())));
            Thread.Sleep(200);

            repo.Value.Received().SendMessages(Arg.Any<NotNullOrWhiteSpaceString>(), Arg.Any<NotNullable<IEnumerable<NotNullOrWhiteSpaceString>>>());
        }
        [Fact]
        public void When_a_message_is_sent_it_sends_a_sentMessage_to_the_mediator()
        {
            List<string> messages = new List<string>(StringExtension.RandomStrings(4));
            List<string> receivedMessages = new List<string>();

            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var mediator = new ServiceEvents();
            mediator.SentIncommingMessageSequence.Subscribe(s=> receivedMessages.Add(s));
            var topic = StringExtension.RandomString();
            var sut = new MessageForwarder(topic, repo, mediator, new TransparentQuota());

            sut.StartForwarding();
            messages.ForEach(m => sut.AddMessage(m));
            Thread.Sleep(200);
            Assert.Equal(messages.Count, receivedMessages.Count);
            messages.ForEach(m => Assert.True(receivedMessages.Contains(m)));
        }

        [Fact]
        public void GetTopic_returns_the_messageForwarder_topic()
        {
            var repo = new NotNullable<IMomRepository>(Substitute.For<IMomRepository>());
            var mediator = new ServiceEvents();
            var topic = StringExtension.RandomString();
            var forwarder = new MessageForwarder(topic, repo, mediator, new TransparentQuota());
            Assert.Equal(forwarder.Topic, topic);
        }

    }
}
