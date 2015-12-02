using Davalor.Base.Library.Serialization;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Security.Encryption;
using Davalor.PortalPaciente.Messages.Answer;
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
    public class AnswerMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public AnswerMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredAnswer_creates_a_new_answer_in_the_database()
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
            var repository = new AnswerRepository(_configuration.TestServer);
            var country = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, country));
        }

        [Fact]
        public void A_ChangedAnswer_modifies_Existing_Answer_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new AnswerRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.PersonId = Guid.NewGuid();
            aggr.AnswerValues.ToList().ForEach(v => v.Type = new Random().Next());

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedAnswer).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var country = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, country));
            
        }

        [Fact]
        public void A_UnregisteredAnswer_removers_Existing_answer_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new AnswerRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredAnswer).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var country = repository.Get(aggr.Id);
            Assert.Null(country);
            
        }


        AnswerAggregate GenerateRandomAggregate()
        {
            var answerId = Guid.NewGuid();
            return new AnswerAggregate
            {
                Id = answerId,
                AppointmentId = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                Type = new Random().Next(),
                TimeStamp = DateTimeOffset.Now,
                AnswerValues = new Collection<AnswerValues>
                {
                    new AnswerValues
                    {
                        Id = Guid.NewGuid(),
                        AnswerId = answerId,
                        TimeStamp = DateTimeExtension.RandomDateTimeOffset(),
                        Type = new Random().Next(),
                        ValueBoolean = StringExtension.RandomString(1),
                        ValueCatalogItemId = Guid.NewGuid(),
                        ValueDecimal = StringExtension.RandomString(3),
                        ValueNumber = StringExtension.RandomString(3)
                    }
                }
            };
        }
        BaseEvent GenerateMessage(AnswerAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<AnswerAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredAnswer).Name,
                Topic = "Country",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }

        
        
    }


}
