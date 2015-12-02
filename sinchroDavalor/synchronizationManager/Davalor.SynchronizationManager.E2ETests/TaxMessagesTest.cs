using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.Tax;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class TaxMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public TaxMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredRegion_creates_a_new_currency_in_the_database()
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
            var repository = new TaxRepository(_configuration.TestServer);
            var tax = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, tax));
        }

        [Fact]
        public void A_ChangedTax_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new TaxRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.NameKeyId = StringExtension.RandomString(10);
            aggr.CountryId = Guid.NewGuid();
            aggr.SapCode = StringExtension.RandomString(2);
            aggr.CurrencyId = Guid.NewGuid();
            aggr.BaseAmount = decimal.Round(Convert.ToDecimal(new Random().NextDouble()), 2 , MidpointRounding.AwayFromZero);
            aggr.Amount = decimal.Round(Convert.ToDecimal(new Random().NextDouble()), 2, MidpointRounding.AwayFromZero);

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedTax).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });


            //5.- Load the saved country
            var tax = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, tax));
        }

        [Fact]
        public void A_UnregisteredTax_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new TaxRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredTax).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var tax = repository.Get(aggr.Id);
            Assert.Null(tax);
        }

        TaxAggregate GenerateRandomAggregate()
        {
            return new TaxAggregate
            {
                Id = Guid.NewGuid(),
                NameKeyId = StringExtension.RandomString(40),
                CountryId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                SapCode = StringExtension.RandomString(2),
                TaxClassServicePrice = StringExtension.RandomString(1),
                TaxClassPartner = StringExtension.RandomString(1),
                Rule = StringExtension.RandomString(1),
                Amount =  decimal.Round(Convert.ToDecimal(new Random().NextDouble()), 2, MidpointRounding.AwayFromZero),
                BaseAmount =  decimal.Round(Convert.ToDecimal(new Random().NextDouble()), 2, MidpointRounding.AwayFromZero),
                BeginPeriod = DateTimeOffset.Now,
                EndPeriod = DateTimeOffset.Now.AddYears(1),
                TimeStamp = DateTimeOffset.Now,

            };
        }
        BaseEvent GenerateMessage(TaxAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<TaxAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredTax).Name,
                Topic = "Tax",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }        
    }


}
