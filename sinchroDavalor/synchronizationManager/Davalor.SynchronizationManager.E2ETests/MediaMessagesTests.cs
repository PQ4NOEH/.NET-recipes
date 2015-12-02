using Davalor.SAP.Messages.Media;
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
    public class MediaMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public MediaMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredMedia_creates_a_new_media_in_the_database()
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
            var repository = new MediaRepository(_configuration.TestServer);
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
            var repository = new MediaRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.SapCode = StringExtension.RandomString(10);
            aggr.MediaMachine.ToList().ForEach(m => m.TimeStamp = DateTimeOffset.Now);
            aggr.MediaServiceLevel.ToList().ForEach(m => m.TimeStamp = DateTimeOffset.Now);
            aggr.MediaDeviceGroup.ToList().ForEach(m => m.Deleted = new Random().Next());

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedMedia).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        [Fact]
        public void A_UnregisteredDevice_modifies_Existing_country_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new MediaRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredMedia).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var service = repository.Get(aggr.Id);
            Assert.Null(service);
        }
        

        MediaAggregate GenerateRandomAggregate()
        {
            var graph = new MediaAggregate
            {
                Id = Guid.NewGuid(),
                Cover = Encoding.UTF8.GetBytes(StringExtension.RandomString()),
                CoverType = StringExtension.RandomString(5),
                LongDescriptionKeyId = StringExtension.RandomString(5),
                NameKeyId = StringExtension.RandomString(5),
                ShortName = StringExtension.RandomString(5),
                NeedsInitialization = new Random().Next(100) < 50,
                Trailer = Encoding.UTF8.GetBytes(StringExtension.RandomString()),
                TrailerType = StringExtension.RandomString(5),
                SapCode =  StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now
            };
            graph.MediaMachine = new Collection<MediaMachine>()
            {
                new MediaMachine
                {
                    Id = Guid.NewGuid(),
                    MachineId = Guid.NewGuid(),
                    MediaId =  graph.Id,
                    TimeStamp = DateTimeOffset.Now
                }
            };
            graph.MediaDeviceGroup = new List<MediaDeviceGroup>
            {
                new MediaDeviceGroup
                {
                    Id = Guid.NewGuid(),
                    MediaId = graph.Id,
                    DeviceGroupId = Guid.NewGuid(),
                    Deleted = new Random().Next(),
                    TimeStamp = DateTimeOffset.Now
                }
            };
            graph.MediaServiceLevel = new Collection<MediaServiceLevel>
            {
                new MediaServiceLevel
                {
                    Id = Guid.NewGuid(),
                    MediaId = graph.Id,
                    ServiceLevelId = Guid.NewGuid(),
                    TimeStamp = DateTimeOffset.Now
                }
            };
            
            return graph;
        }
        BaseEvent GenerateMessage(MediaAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<MediaAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredMedia).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
