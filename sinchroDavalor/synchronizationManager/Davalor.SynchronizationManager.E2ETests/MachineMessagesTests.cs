using Davalor.SAP.Messages.Machine;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System.Collections.ObjectModel;
using Davalor.Toolkit.Extensions;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Security.Encryption;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class MachineMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public MachineMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredMachine_creates_a_new_Machine_in_the_database()
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
            var repository = new MachineRepository(_configuration.TestServer);
            var service = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }
        [Fact]
        public void A_ChangedMachine_modifies_Existing_Machine_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new MachineRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.DescriptionKeyId = StringExtension.RandomString(10);
            aggr.MachineGroup.SapCode = StringExtension.RandomString(10);
            aggr.MachinePrinter.First().Function = new Random().Next();

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedMachine).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        [Fact]
        public void A_UnregisteredMachine_removes_Existing_machine_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new MachineRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredMachine).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var service = repository.Get(aggr.Id);
            Assert.Null(service);
        }
        MachineAggregate GenerateRandomAggregate()
        {
            var graph = new MachineAggregate
            {
                Id = Guid.NewGuid(),
                DeviceIdentifier = StringExtension.RandomString(10),
                Deleted = new Random().Next(),
                Ip = StringExtension.RandomString(10),
                SortOrder = new Random().Next(),
                StreamingPort = new Random().Next(),
                PortState = new Random().Next(),
                PartnerId = Guid.NewGuid(),
                Port = new Random().Next(),
                UserFriendlyName = StringExtension.RandomString(),
                DescriptionKeyId = StringExtension.RandomString(10),
                SerialNumber = StringExtension.RandomString(10),
                SapCode =  StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now,
                MachineGroupId = Guid.NewGuid()
            };
            graph.MachineGroup = new MachineGroup
            {
                Id = graph.MachineGroupId,
                Deleted = new Random().Next(),
                NameKeyId = StringExtension.RandomString(10),
                SapCode = "SAP-CODE",
                TimeStamp = DateTimeOffset.Now
            };
            graph.MachinePrinter = new Collection<MachinePrinter>
            {
                new MachinePrinter
                {
                    Id = Guid.NewGuid(),
                    MachineId = graph.Id,
                    PrinterId = Guid.NewGuid(),
                    Function = new Random().Next(),
                    TimeStamp = DateTimeOffset.Now
                }
            };
            
            return graph;
        }
        BaseEvent GenerateMessage(MachineAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<MachineAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredMachine).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
