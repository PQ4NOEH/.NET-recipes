using Davalor.Framework.Core.Messaging;
using Davalor.Framework.Core.Testing;
using Davalor.MomProxy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.UnitTest
{
    public class HostMediatorSpec
    {
        [Fact]
        public void When_SavedIncommingMessage_it_notifies_subscriptor()
        {
            List<string> messages = new List<string>();
            List<string> savedMessages = new List<string>();
            (4).Times(new Action(() =>
            {
                messages.Add(StringExtensions.RandomString() );
            }));
            var sut = new HostMediator();
            sut.SavedIncommingMessageSequence.Subscribe(savedMessages.Add);
            messages.ForEach(sut.SavedIncommingMessage);
            Assert.Equal(messages.Count, savedMessages.Count);
            messages.ForEach(m => Assert.True(savedMessages.Contains(m)));

        }
        [Fact]
        public void When_SentMessage_it_notifies_subscriptor()
        {
            List<string> messages = new List<string>(StringExtensions.RandomStrings(6));
            List<string> sentMessages = new List<string>();
            var sut = new HostMediator();
            sut.SentIncommingMessageSequence.Subscribe(sentMessages.Add);
            messages.ForEach(m => sut.SentIncommingMessage(m));
            Assert.Equal(messages.Count, sentMessages.Count);
            messages.ForEach(m => Assert.True(sentMessages.Contains(m)));
        }
        [Fact]
        public void When_UndeserializableMessage_it_notifies_subscriptor()
        {
            List<UndeserializableMessage> messages = new List<UndeserializableMessage>();
            List<UndeserializableMessage> undeserializableMessages = new List<UndeserializableMessage>();
            (6).Times(new Action(() =>
            {
                messages.Add(new UndeserializableMessage 
                { 
                    Message = StringExtensions.RandomString(),
                    Ex = new ArgumentException(StringExtensions.RandomString())
                });
            }));
            var sut = new HostMediator();
            sut.UndeserializableIncommingMessageSequence.Subscribe(undeserializableMessages.Add);
            messages.ForEach(m => sut.UndeserializableIncommingMessage(m));
            Assert.Equal(messages.Count, undeserializableMessages.Count);
            messages.ForEach(m =>
            { 
                var msg = undeserializableMessages.Where(u=> u.Message == m.Message).FirstOrDefault();
                Assert.NotNull(msg);
                Assert.Equal(m.Ex.GetType(), msg.Ex.GetType());
                Assert.Equal(m.Ex.Message, msg.Ex.Message);
            });
        }   
        [Fact]
        public void Instance_will_lazily_create_a_unique_hostMediatorInstance()
        {
            Assert.False(HostMediator.Instance.IsValueCreated);
            var mediator = HostMediator.Instance.Value;
            Assert.True(HostMediator.Instance.IsValueCreated);
            (3).Times(new Action(() =>
            {
                Assert.Same(mediator, HostMediator.Instance.Value);
            }));
        }
    }
}
