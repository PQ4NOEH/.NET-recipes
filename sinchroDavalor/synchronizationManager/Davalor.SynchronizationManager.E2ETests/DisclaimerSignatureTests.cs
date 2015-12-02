using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.PortalPaciente.Messages.DisclaimerSignature;
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
    public class DisclaimerSignatureTests
    {
        readonly TestHostConfiguration _configuration;
        public DisclaimerSignatureTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_signedDisclaimer_creates_a_new_DisclaimerSignature_in_the_database()
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
            var repository = new DisclaimerSignatureRepository(_configuration.TestServer);
            var disclaimer = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, disclaimer));
        }


        DisclaimerSignatureAggregate GenerateRandomAggregate()
        {
            var signatureId = Guid.NewGuid();
            return new DisclaimerSignatureAggregate
            {
                Id = Guid.NewGuid(),
                State = new Random().Next(),
                DisclaimerId = Guid.NewGuid(),
                SignatureId = signatureId,
                PatientId = Guid.NewGuid(),
                SignedDate = DateTimeExtension.RandomDateTimeOffset(),
                TimeStamp = DateTimeOffset.Now,
                Signature = new Signature
                {
                    Id = signatureId,
                    SafekeepingIdentifier = StringExtension.RandomString(8),
                    TimeStamp = DateTimeOffset.Now
                }
            };
        }
        BaseEvent GenerateMessage(DisclaimerSignatureAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<DisclaimerSignatureAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(SignedDisclaimer).Name,
                Topic = "DisclaimerSignature",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }

        
        
    }


}
