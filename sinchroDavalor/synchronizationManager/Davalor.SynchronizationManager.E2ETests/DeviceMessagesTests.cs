using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.SAP.Messages.Device;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class DeviceMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public DeviceMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredDevice_creates_a_new_currency_in_the_database()
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
            var repository = new DeviceRepository(_configuration.TestServer);
            var service = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }
        [Fact]
        public void A_ChangedDevice_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new DeviceRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.DescriptionKeyId = StringExtension.RandomString(10);
            aggr.DeviceGroup.SapCode = StringExtension.RandomString(10);
            aggr.DeviceGroup.DeviceType.NameKeyId = StringExtension.RandomString(10);

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedDevice).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        [Fact]
        public void A_UnregisteredDevice_removes_Existing_device_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new DeviceRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredDevice).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var service = repository.Get(aggr.Id);
            Assert.Null(service);
        }
        
        DeviceAggregate GenerateRandomAggregate()
        {
            var graph = new DeviceAggregate
            {
                Id = Guid.NewGuid(),
                DescriptionKeyId = StringExtension.RandomString(33),
                SerialNumber = StringExtension.RandomString(17),
                MachineId = Guid.NewGuid(),
                DeviceGroupId = Guid.NewGuid(),
                SapCode = StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now
            };
            graph.DeviceGroup = new DeviceGroup
            {
                Id = graph.DeviceGroupId,
                NameKeyId = StringExtension.RandomString(30),
                DeviceTypeId = Guid.NewGuid(),
                SapCode = StringExtension.RandomString(19),
                TimeStamp = DateTimeOffset.Now
            };
            graph.DeviceGroup.DeviceType = new DeviceType
            {
                Id = graph.DeviceGroup.DeviceTypeId,
                NameKeyId = StringExtension.RandomString(30),
                SapCode = StringExtension.RandomString(12),
                TimeStamp = DateTimeOffset.Now
            };
            
            return graph;
        }
        BaseEvent GenerateMessage(DeviceAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<DeviceAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredDevice).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
