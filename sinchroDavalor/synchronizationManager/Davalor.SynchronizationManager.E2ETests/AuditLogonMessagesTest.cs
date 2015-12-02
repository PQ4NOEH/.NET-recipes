using Davalor.VisionLocal.Messages.AuditLogon;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using System;
using Xunit;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Security.Encryption;
using Davalor.Toolkit.Extensions;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class AuditLogonMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public AuditLogonMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        //[Fact]
        public void A_RegisteredAuditLogon_creates_a_new_currency_in_the_database()
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
            var repository = new AuditLogonRepository(_configuration.TestServer);
            var title = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.Equal(aggr.Id, title.Id);
            Assert.Equal(aggr.UserName, title.UserName);
            Assert.Equal(aggr.Ip, title.Ip);
            Assert.Equal(aggr.Access, title.Access);
            Assert.Equal(aggr.AccessDate, title.AccessDate);
            Assert.Equal(aggr.PartnerId, title.PartnerId);
            Assert.Equal(aggr.TimeStamp.Second, title.TimeStamp.Second);
        }

        AuditLogonAggregate GenerateRandomAggregate()
        {
            return new AuditLogonAggregate
            {
                Id = Guid.NewGuid(),
                UserName = StringExtension.RandomString(40),
                Ip = StringExtension.RandomString(40),
                Access = StringExtension.RandomString(40),
                AccessDate = DateTimeOffset.Now,
                PartnerId = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
        BaseEvent GenerateMessage(AuditLogonAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<AuditLogonAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredAuditLogon).Name,
                Topic = "AuditLogon",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }        
    }


}
