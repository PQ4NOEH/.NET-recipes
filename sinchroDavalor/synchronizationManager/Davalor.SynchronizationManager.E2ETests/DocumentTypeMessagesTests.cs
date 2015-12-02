using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.DocumentType;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class DocumentTypeMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public DocumentTypeMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredDocumenType_creates_a_new_currency_in_the_database()
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
            var repository = new DocumentTypeRepository(_configuration.TestServer);
            var documentType = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, documentType));
        }

        [Fact]
        public void A_ChangedDocumenType_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new DocumentTypeRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.CountryId = Guid.NewGuid();
            aggr.NameKeyId = StringExtension.RandomString(10);
            aggr.SapCode = StringExtension.RandomString(2);

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedDocumentType).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var documentType = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, documentType));
        }

        [Fact]
        public void A_UnregisteredDocumenType_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new DocumentTypeRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredDocumentType).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var country = repository.Get(aggr.Id);
            Assert.Null(country);
        }

        DocumentTypeAggregate GenerateRandomAggregate()
        {
            return new DocumentTypeAggregate
            {
                Id = Guid.NewGuid(),
                NameKeyId = StringExtension.RandomString(40),
                CountryId = Guid.NewGuid(),
                SapCode = StringExtension.RandomString(2),
                TimeStamp = DateTimeOffset.Now
            };
        }
        BaseEvent GenerateMessage(DocumentTypeAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<DocumentTypeAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredDocumentType).Name,
                Topic = "DocumentType",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        } 
    }


}
