using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.PortalPaciente.Messages.Payable;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class PayableMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public PayableMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredPayable_creates_a_new_Payable_in_the_database()
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
            var repository = new PayableRepository(_configuration.TestServer);
            var Payable = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, Payable));
        }

        [Fact]
        public void A_ChangedPayable_modifies_Existing_Payable_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PayableRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.CurrencyId = Guid.NewGuid();
            aggr.PaymentTransaction.ToList().ForEach(t =>
                {
                    t.PartnerId = Guid.NewGuid();
                    t.Payment.PaymentType = new Random().Next();
                    t.Payment.PaymentMpos.OperationNumber = StringExtension.RandomString(5);
                    t.PaymentTaxTransaction.ToList().ForEach(tt =>
                        {
                            tt.TaxId = Guid.NewGuid();
                        });
                });

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedPayable).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var country = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, country));
            
        }

        [Fact]
        public void A_UnregisteredPayable_modifies_Existing_Payable_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PayableRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredPayable).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var country = repository.Get(aggr.Id);
            Assert.Null(country);
            
        }

        PayableAggregate GenerateRandomAggregate()
        {
            var payableId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var paymentTransactionId = Guid.NewGuid();
            return new PayableAggregate
            {
                Id = payableId,
                CurrencyId = Guid.NewGuid(),
                PaymentStatus = new Random().Next(),
                Price = (float)new Random().NextDouble(),
                TimeStamp = DateTimeOffset.Now,
                PaymentTransaction = new Collection<PaymentTransaction>
                {
                    new PaymentTransaction
                    {
                        Id = paymentTransactionId,
                        Amount = decimal.Round((decimal)new Random().NextDouble(), 2, MidpointRounding.AwayFromZero),
                        CurrencyId = Guid.NewGuid(),
                        PartnerId = Guid.NewGuid(),
                        PayableId = payableId,
                        PaymentId = paymentId,
                        TimeStamp = DateTimeExtension.RandomDateTimeOffset(),
                        Payment = new PaymentEntity
                        {
                            Id = paymentId,
                            Amount = decimal.Round((decimal)new Random().NextDouble(), 2, MidpointRounding.AwayFromZero),
                            CurrencyId = Guid.NewGuid(),
                            FreeSessionReasonId = Guid.NewGuid(),
                            GatewayId = Guid.NewGuid(),
                            OperationType = new Random().Next(),
                            PatientId = Guid.NewGuid(),
                            PaymentDate = DateTimeExtension.RandomDateTime(),
                            PaymentType = new Random().Next(),
                            ProcessingState = new Random().Next(),
                            RequestDate = DateTimeExtension.RandomDateTime(),
                            TimeStamp = DateTimeExtension.RandomDateTimeOffset(),
                            PaymentMpos = new PaymentMpos
                            {
                                Id = paymentId,
                                AuthNumber = StringExtension.RandomString(5),
                                BankingNumber = StringExtension.RandomString(5),
                                OperationNumber = StringExtension.RandomString(5),
                                Sequence = StringExtension.RandomString(5),
                                TimeStamp =  DateTimeExtension.RandomDateTimeOffset()
                            }
                        },
                        PaymentTaxTransaction = new Collection<PaymentTaxTransaction>
                        {
                            new PaymentTaxTransaction
                            {
                                Id = Guid.NewGuid(),
                                Amount = decimal.Round((decimal)new Random().NextDouble(), 2, MidpointRounding.AwayFromZero),
                                NameKeyId = StringExtension.RandomString(5),
                                TaxId = Guid.NewGuid(),
                                PaymentTransactionId = paymentTransactionId,
                                TimeStamp = DateTimeExtension.RandomDateTimeOffset()
                            }
                        }
                    }
                }
            };
        }
        BaseEvent GenerateMessage(PayableAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<PayableAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredPayable).Name,
                Topic = "Payable",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }

        
        
    }


}
