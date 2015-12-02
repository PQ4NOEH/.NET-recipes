using Davalor.PortalPaciente.Messages.Patient;
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
    public class PatientMessagesTests
    {
        readonly TestHostConfiguration _configuration;
        public PatientMessagesTests()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_RegisteredPatient_creates_a_new_Patient_in_the_database()
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
            var repository = new PatientRepository(_configuration.TestServer);
            var service = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }
        [Fact]
        public void A_ChangedPatient_modifies_Existing_Patient_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PatientRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //3.- Change the aggregate
            aggr.SapCode = StringExtension.RandomString();
            aggr.Person.Phone1 = StringExtension.RandomString(3);
            aggr.Person.PersonLocation.First().LocationId = Guid.NewGuid();
            aggr.User.RegistrationCode = StringExtension.RandomString(5);

            //4.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(ChangedPatient).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            //5.- Load the saved country
            var service = repository.Get(aggr.Id);
            //6.- Check equality
            Assert.True(ObjectExtension.AreEqual(aggr, service));
        }

        [Fact]
        public void A_UnregisteredPatient_removes_Existing_Patient_in_the_database()
        {
            var bootStrapper = new BootStrapper();
            bootStrapper.StartServices();
            var serviceEvents = bootStrapper.GetService<IServiceEvents>();
            //1.- Create message
            var aggr = GenerateRandomAggregate();

            //2.- Create the tuple in the database
            var repository = new PatientRepository(_configuration.TestServer);
            repository.Insert(aggr);

            //2.- Emit message
            var message = GenerateMessage(aggr);
            message.MessageType = typeof(UnregisteredPatient).Name;
            serviceEvents.AddIncommingEvent(new IncommingEvent { @event = message });

            var service = repository.Get(aggr.Id);
            Assert.Null(service);
        }

        PatientAggregate GenerateRandomAggregate()
        {
            var graph = new PatientAggregate
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SapCode =  StringExtension.RandomString(10),
                TimeStamp = DateTimeOffset.Now
            };
            graph.User = new User
            {
                Id = graph.UserId.Value,
                Active = new Random().Next(),
                Email = StringExtension.RandomString(),
                ForgotPasswordCode = StringExtension.RandomString(5),
                ForgotPasswordRequest = DateTimeExtension.RandomDateTimeOffset(),
                Hash = StringExtension.RandomString(5),
                LanguageId = Guid.NewGuid(),
                LastChangePassword = DateTimeExtension.RandomDateTimeOffset(),
                LastLogon = DateTimeExtension.RandomDateTimeOffset(),
                Locked = new Random().Next(),
                NewEmail = StringExtension.RandomString(5),
                NewEmailCode = StringExtension.RandomString(5),
                NewEmailRequest = DateTimeExtension.RandomDateTimeOffset(),
                RecordDeleted = new Random().Next(),
                RecordDeletedDate = DateTimeExtension.RandomDateTimeOffset(),
                RegistrationDate = DateTimeExtension.RandomDateTimeOffset(),
                RegistrationCode = StringExtension.RandomString(5),
                RetryCount = new Random().Next(),
                TokenHash = StringExtension.RandomString(5),
                UserName = StringExtension.RandomString(5),
                TimeStamp = DateTimeOffset.Now
            };

            graph.Person = new Person
            {
                Id = graph.PersonId,
                BirthDate = DateTimeExtension.RandomDateTime(),
                CurrencyId = Guid.NewGuid(),
                Deleted = new Random().Next(),
                DocumentIdentifier = StringExtension.RandomString(5),
                DocumentTypeId = Guid.NewGuid(),
                Email = StringExtension.RandomString(5),
                Gender = new Random().Next(),
                Name = StringExtension.RandomString(5),
                LanguageId = Guid.NewGuid(),
                NationalityId = Guid.NewGuid(),
                Phone1 = StringExtension.RandomString(5),
                Phone2 = StringExtension.RandomString(5),
                Surname1 = StringExtension.RandomString(5),
                Surname2 = StringExtension.RandomString(5),
                TitleId = Guid.NewGuid(),
                TimeStamp = DateTimeExtension.RandomDateTime(),
                PersonLocation = new Collection<PersonLocation>
                {
                    new PersonLocation
                    {
                        Id = Guid.NewGuid(),
                        LocationId = Guid.NewGuid(),
                        PersonId = graph.PersonId,
                        TimeStamp= DateTimeExtension.RandomDateTime()
                    }
                }
            };
            
            return graph;
        }
        BaseEvent GenerateMessage(PatientAggregate aggregate)
        {
            var serializedAggregate = new JsonSerializer().Serialize<PatientAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(RegisteredPatient).Name,
                Topic = "Service",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }
    }
}
