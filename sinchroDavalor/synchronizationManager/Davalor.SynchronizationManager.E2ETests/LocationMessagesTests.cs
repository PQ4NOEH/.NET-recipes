using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.Location;
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
    public class LocationMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public LocationMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredLocation_creates_a_new_Location_in_the_database()
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
            var repository = new LocationRepository(_configuration.TestServer);
            var location = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, location));
        }

        [Fact]
        public void A_ChangedLocation_modifies_Existing_Location_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new LocationRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.PostalCode = StringExtension.RandomString(10);
            aggr.CountryId = Guid.NewGuid();

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedLocation).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var location = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, location));
        }

        [Fact]
        public void A_UnregisteredLocation_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new LocationRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredLocation).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var location = repository.Get(aggr.Id);
            Assert.Null(location);
            
        }


        LocationAggregate GenerateRandomAggregate()
        {
            return new LocationAggregate
            {
                Id = Guid.NewGuid(),
                City = StringExtension.RandomString(20),
                Street = StringExtension.RandomString(20),
                PostalCode = StringExtension.RandomString(10),
                Longitude = new Random().NextDouble(),
                Latitude = new Random().NextDouble(),
                CountryId = Guid.NewGuid(),
                RegionId = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
        BaseEvent GenerateMessage(LocationAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<LocationAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredLocation).Name,
                Topic = "Location",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }

        
    }


}
