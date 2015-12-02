using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.PortalPaciente.Messages.Disclaimer;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class DisclaimerMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public DisclaimerMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredDisclaimer_creates_a_new_Disclaimer_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();
            var message = GenerateMessage(aggr);
            //2.- Emit message
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });
            //3.- Load the saved country
            var repository = new DisclaimerRepository(_configuration.TestServer);
            var disclaimer = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, disclaimer));
        }

        


        DisclaimerAggregate GenerateRandomAggregate()
        {
            return new DisclaimerAggregate
            {
                Id = Guid.NewGuid(),
                Version = new Random().Next(),
                Code = StringExtension.RandomString(2),
                TextKeyId = StringExtension.RandomString(10),
                LinkKeyId = StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now
            };
        }
        BaseEvent GenerateMessage(DisclaimerAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<DisclaimerAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredDisclaimer).Name,
                Topic = "Country",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }

        
        
    }


}
