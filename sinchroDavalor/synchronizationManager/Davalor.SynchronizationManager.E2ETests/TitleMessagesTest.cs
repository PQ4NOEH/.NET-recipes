using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.Title;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class TitleMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public TitleMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredTitle_creates_a_new_currency_in_the_database()
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
            var repository = new TitleRepository(_configuration.TestServer);
            var title = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, title));
        }

        [Fact]
        public void A_ChangedTitle_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new TitleRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.NameKeyId = StringExtension.RandomString(10);
            aggr.SapCode = StringExtension.RandomString(2);
            aggr.Deleted = new Random().Next();

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedTitle).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });


            //5.- Load the saved country
            var title = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, title));
        }

        [Fact]
        public void A_UnregisteredTitle_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new TitleRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredTitle).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var tax = repository.Get(aggr.Id);
            Assert.Null(tax);
        }

        TitleAggregate GenerateRandomAggregate()
        {
            return new TitleAggregate
            {
                Id = Guid.NewGuid(),
                NameKeyId = StringExtension.RandomString(40),
                Deleted = new Random().Next(),
                SapCode = StringExtension.RandomString(2),
                
                TimeStamp = DateTimeOffset.Now,

            };
        }
        BaseEvent GenerateMessage(TitleAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<TitleAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredTitle).Name,
                Topic = "Title",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }        
    }


}
