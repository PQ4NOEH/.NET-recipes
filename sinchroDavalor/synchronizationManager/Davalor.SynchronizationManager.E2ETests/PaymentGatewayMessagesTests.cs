using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.PaymentGateway;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class PaymentGatewayMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public PaymentGatewayMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredGateway_creates_a_new_Gateway_in_the_database()
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
            var repository = new PaymentGatewayRepository(_configuration.TestServer);
            var gateway = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, gateway));
        }

        [Fact]
        public void A_ChangedGateway_modifies_Existing_Gateway_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PaymentGatewayRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.GatewayDescription = StringExtension.RandomString(20);
            aggr.GatewayByCountry.ToList().ForEach(e => e.CountryId = Guid.NewGuid());

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedPaymentGateway).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var gateway = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, gateway));
        }

        [Fact]
        public void A_UnregisteredCountry_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PaymentGatewayRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredPaymentGateway).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var country = repository.Get(aggr.Id);
            Assert.Null(country);
            
        }
        GatewayAggregate GenerateRandomAggregate()
        {
            var result =  new GatewayAggregate
            {
                Id = Guid.NewGuid(),
                GatewayDescription = StringExtension.RandomString(20),
                GatewayPlatformType = new Random().Next(),
                GatewayType = new Random().Next(),
                GatewayTypeName = StringExtension.RandomString(20),
                TimeStamp = DateTimeOffset.Now
            };
            result.GatewayByCountry = new Collection<GatewayByCountry>
                {
                    new GatewayByCountry
                    {
                        Id = Guid.NewGuid(),
                        GatewayId = result.Id,
                        CountryId = Guid.NewGuid(),
                        TimeStamp = DateTimeOffset.Now
                    }
                };
            return result;
        }
        BaseEvent GenerateMessage(GatewayAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<GatewayAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredPaymentGateway).Name,
                Topic = "PaymentGateway",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }


}
