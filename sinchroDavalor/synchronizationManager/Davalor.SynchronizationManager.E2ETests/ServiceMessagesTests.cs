using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.Service;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class ServiceMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public ServiceMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredService_creates_a_new_currency_in_the_database()
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
            var repository = new ServiceRepository(_configuration.TestServer);
            var service = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }
        [Fact]
        public void A_ChangedService_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new ServiceRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.NameKeyId = StringExtension.RandomString(10);
            aggr.ServiceType.SapCode = StringExtension.RandomString(10);
            aggr.ServiceLevel.First().SapCode = StringExtension.RandomString(10);
            aggr.ServiceLevel.First().ServicePrice.First().Price = (float)new Random().NextDouble();

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedService).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        [Fact]
        public void A_UnregisteredService_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new ServiceRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredService).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var service = repository.Get(aggr.Id);
            Assert.Null(service);
        }

        ServiceAggregate GenerateRandomAggregate()
        {
            var graph = new ServiceAggregate
            {
                Id = Guid.NewGuid(),
                NameKeyId = StringExtension.RandomString(20),
                LongDescriptionKeyId = StringExtension.RandomString(33),
                Cover = Encoding.UTF8.GetBytes(StringExtension.RandomString()),
                CoverType = StringExtension.RandomString(4),
                Deleted = new Random().Next(),
                ServiceTypeId = Guid.NewGuid(),
                DecisionTreeId = Guid.NewGuid(),
                SapCode =  StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now
            };
            graph.ServiceType = new ServiceType
            {
                Id = graph.ServiceTypeId,
                NameKeyId = StringExtension.RandomString(30),
                LongDescriptionKeyId = StringExtension.RandomString(30),
                Length = new Random().Next(),
                Type = new Random().Next(),
                Deleted = new Random().Next(),
                SapCode = StringExtension.RandomString(19),
                TimeStamp = DateTimeOffset.Now
            };
            graph.ServiceLevel = new List<ServiceLevel>
            {
                new ServiceLevel
                {
                    Id = Guid.NewGuid(),
                    NameKeyId = StringExtension.RandomString(30),
                    LongDescriptionKeyId  = StringExtension.RandomString(30),
                    Deleted = new Random().Next(),
                    ServiceId =  graph.Id,
                    SapCode  = StringExtension.RandomString(17),
                    TimeStamp = DateTimeOffset.Now,
                    ServicePrice = new List<ServicePrice>()
                }
            };
            graph.ServiceLevel.First().ServicePrice.Add(new ServicePrice
                {
                    Id = Guid.NewGuid(),
                    ServiceLevelId = graph.ServiceLevel.First().Id,
                    BeginPeriod = DateTime.Now.AddYears(-2),
                    EndPeriod = DateTime.Now,
                    CountryId = Guid.NewGuid(),
                    CurrencyId = Guid.NewGuid(),
                    Price = (float)new Random().NextDouble(),
                    SapCode = StringExtension.RandomString(30),
                    TimeStamp = DateTimeOffset.Now,
                    TaxClass = StringExtension.RandomString(1),
                });
            return graph;
        }
        BaseEvent GenerateMessage(ServiceAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<ServiceAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredService).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
