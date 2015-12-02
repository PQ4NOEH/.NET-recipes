
using Davalor.Base.Messaging.Contracts;
using Davalor.MomProxy.Domain;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Davalor.MomProxy.UnitTest
{
    public class ServiceEventsSpec
    {
        [Fact]
        public void When_SavedIncommingMessage_it_notifies_subscriptor()
        {
            List<string> messages = new List<string>();
            List<string> savedMessages = new List<string>();
            (4).Times(new Action(() =>
            {
                messages.Add(StringExtension.RandomString() );
            }));
            var sut = new ServiceEvents();
            sut.SavedIncommingMessageSequence.Subscribe(s => savedMessages.Add(s.Value));
            messages.ForEach(s=>sut.SavedIncommingMessage(s));
            Assert.Equal(messages.Count, savedMessages.Count);
            messages.ForEach(m => Assert.True(savedMessages.Contains(m)));

        }
        [Fact]
        public void When_SentMessage_it_notifies_subscriptor()
        {
            List<string> messages = new List<string>(StringExtension.RandomStrings(6));
            List<string> sentMessages = new List<string>();
            var sut = new ServiceEvents();
            sut.SentIncommingMessageSequence.Subscribe(s=> sentMessages.Add(s));
            messages.ForEach(m => sut.SentIncommingMessage(m));
            Assert.Equal(messages.Count, sentMessages.Count);
            messages.ForEach(m => Assert.True(sentMessages.Contains(m)));
        }

        [Fact]
        public void When_DeletedMessage_it_notifies_subscriptor()
        {
            List<string> messages = new List<string>(StringExtension.RandomStrings(6));
            List<string> deletedMessages = new List<string>();
            var sut = new ServiceEvents();
            sut.DeletedIncommingMessageSequence.Subscribe(s => deletedMessages.Add(s));
            messages.ForEach(m => sut.DeletedIncommingMessage(m));
            Assert.Equal(messages.Count, deletedMessages.Count);
            messages.ForEach(m => Assert.True(deletedMessages.Contains(m)));
        }

        [Fact]
        public void When_receivesMessage_it_notifies_subscriptor()
        {
            List<BaseEvent> messages = new List<BaseEvent>();
            (4).Times(new Action(()=> {
                messages.Add(new BaseEvent
                {
                    EventID = Guid.NewGuid(),
                    Topic = StringExtension.RandomString()
                });
            }));
            List<BaseEvent> receivedMessages = new List<BaseEvent>();
            var sut = new ServiceEvents();
            sut.ReceivedMessageSequence.Subscribe(s => receivedMessages.Add(s));
            messages.ForEach(s=> sut.ReceivedMessage(s));
            Assert.Equal(messages.Count, receivedMessages.Count);
            messages.ForEach(m => Assert.True(receivedMessages.Contains(m)));
        }
        [Fact]
        public void When_ReceivedInvalidMessage_it_notifies_subscriptor()
        {
            List<BaseEvent> messages = new List<BaseEvent>();
            (4).Times(new Action(() =>
            {
                messages.Add(new BaseEvent
                {
                    EventID = Guid.NewGuid(),
                    Topic = StringExtension.RandomString()
                });
            }));
            List<BaseEvent> receivedInvalidMessages = new List<BaseEvent>();
            var sut = new ServiceEvents();
            sut.ReceivedInvalidMessageSequence.Subscribe(s => receivedInvalidMessages.Add(s));
            messages.ForEach(s => sut.ReceivedInvalidMessage(s));
            Assert.Equal(messages.Count, receivedInvalidMessages.Count);
            messages.ForEach(m => Assert.True(receivedInvalidMessages.Contains(m)));
        }      
        [Fact]
        public void Instance_will_lazily_create_a_unique_hostMediatorInstance()
        {
            Assert.False(ServiceEvents.Instance.IsValueCreated);
            var mediator = ServiceEvents.Instance.Value;
            Assert.True(ServiceEvents.Instance.IsValueCreated);
            (3).Times(new Action(() =>
            {
                Assert.Same(mediator, ServiceEvents.Instance.Value);
            }));
        }
    }
}
