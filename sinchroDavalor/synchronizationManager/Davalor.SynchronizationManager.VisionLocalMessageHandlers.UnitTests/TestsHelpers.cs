using Davalor.Base.Messaging.Contracts;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public static class TestsHelpers
    {
        public static IncommingEvent GenerateRandomMessage(string MessageType)
        {
            return new IncommingEvent
            {
                @event = new BaseEvent
                {
                    Aggregate = StringExtension.RandomString(),
                    EventID = Guid.NewGuid(),
                    MessageOriginator = StringExtension.RandomString(),
                    MessageType = MessageType,
                    Topic = StringExtension.RandomString()
                }
            };
        }
    }
}
