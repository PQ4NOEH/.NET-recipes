using Davalor.SAP.Messages.Partner;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System.Text;
using System.Collections.ObjectModel;
using Davalor.Toolkit.Extensions;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Security.Encryption;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class PartnerMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public PartnerMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredPartner_creates_a_new_Partner_in_the_database()
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
            var repository = new PartnerRepository(_configuration.TestServer);
            var service = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }
        [Fact]
        public void A_ChangedPartner_modifies_Existing_Partner_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PartnerRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.SapCode = StringExtension.RandomString(10);
            aggr.PartnerChain.SapCode = StringExtension.RandomString();

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedPartner).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        PartnerAggregate GenerateRandomAggregate()
        {
            var graph = new PartnerAggregate
            {
                Id = Guid.NewGuid(),
                Name = StringExtension.RandomString(),
                Phone = StringExtension.RandomString(),
                Email = StringExtension.RandomString(),
                Url = StringExtension.RandomString(),
                Deleted = new Random().Next(),
                TimeZoneId = Guid.NewGuid(),
                LanguageId = Guid.NewGuid(),
                LocationId = Guid.NewGuid(),
                PartnerChainId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                TaxClass = StringExtension.RandomString(1),
                SapCode = StringExtension.RandomString(),
                TimeStamp =  DateTimeOffset.Now
            };
            graph.PartnerChain = new PartnerChain
            {
                Id = graph.PartnerChainId.Value,
                Name = StringExtension.RandomString(),
                SapCode = StringExtension.RandomString(),
                TimeStamp = DateTimeOffset.Now
            };
            
            return graph;
        }
        BaseEvent GenerateMessage(PartnerAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<PartnerAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredPartner).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
