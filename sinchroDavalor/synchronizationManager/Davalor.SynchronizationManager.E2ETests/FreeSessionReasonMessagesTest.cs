using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.FreeSessionReason;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class FreeSessionReasonMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public FreeSessionReasonMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredFreeSessionReason_creates_a_new_currency_in_the_database()
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
            var repository = new FreeSessionReasonRepository(_configuration.TestServer);
            var freeSessionReason = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, freeSessionReason));
        }

        [Fact]
        public void A_ChangedFreeSessionReason_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new FreeSessionReasonRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.NameKeyId = StringExtension.RandomString(10);
            aggr.SapCode = StringExtension.RandomString(2);

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedFreeSessionReason).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var freeSessionReason = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, freeSessionReason));
        }

        [Fact]
        public void A_UnregisteredFreeSessionReason_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new FreeSessionReasonRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredFreeSessionReason).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var country = repository.Get(aggr.Id);
            Assert.Null(country);
        }

        FreeSessionReasonAggregate GenerateRandomAggregate()
        {
            return new FreeSessionReasonAggregate
            {
                Id = Guid.NewGuid(),
                NameKeyId = StringExtension.RandomString(40),
                SapCode = StringExtension.RandomString(2),
                TimeStamp = DateTimeOffset.Now
            };
        }
        BaseEvent GenerateMessage(FreeSessionReasonAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<FreeSessionReasonAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredFreeSessionReason).Name,
                Topic = "FreeSessionReason",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }        
    }


}
