using Davalor.VisionLocal.Messages.Session;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.E2ETests.Database;
using Davalor.SynchronizationManager.Host;
using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using Davalor.Toolkit.Extensions;
using Davalor.Base.Messaging.Contracts;
using Davalor.Base.Library.Serialization;
using Davalor.Base.Security.Encryption;

namespace Davalor.SynchronizationManager.E2ETests
{
    [Collection("E2ETestsCollection")]
    public class SessionMessagesTest
    {
        readonly TestHostConfiguration _configuration;
        public SessionMessagesTest()
        {
            _configuration = new TestHostConfiguration();
        }

        [Fact]
        public void A_FinisedEvaSession_creates_a_new_Session_in_the_database()
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
            var repository = new SessionRepository(_configuration.TestServer);
            var session = repository.Get(aggr.Id);
            //4.- Check equality
            Assert.Equal(aggr.Id, session.Id);
            Assert.Equal(aggr.CommissionId, session.CommissionId);
            Assert.Equal(aggr.EndTime.Value.Second, session.EndTime.Value.Second);
            Assert.Equal(aggr.InitialTime.Value.Second, session.InitialTime.Value.Second);
            Assert.Equal(aggr.InvoiceId, session.InvoiceId);
            Assert.Equal(aggr.InterpretationDone, session.InterpretationDone);
            Assert.Equal(aggr.SignedDate.Value.Second, session.SignedDate.Value.Second);
            Assert.Equal(aggr.GuardianId, session.GuardianId);
            Assert.Equal(aggr.PartnerId, session.PartnerId);
            Assert.Equal(aggr.PatientId, session.PatientId);
            Assert.Equal(aggr.ServiceLevelId, session.ServiceLevelId);
            Assert.Equal(aggr.ServiceId, session.ServiceId);
            Assert.Equal(aggr.MediaId, session.MediaId);
            Assert.Equal(aggr.MachineId, session.MachineId);
            Assert.Equal(aggr.PayableId, session.PayableId);
            Assert.Equal(aggr.CommissionId, session.CommissionId);
            Assert.Equal(aggr.TimeStamp.Second, session.TimeStamp.Second);

            var appoinment = session.Appointment.First();
            Assert.Equal(appoinment.Id, aggr.Appointment.First().Id);
            Assert.Equal(appoinment.InitialTime.Second, aggr.Appointment.First().InitialTime.Second);
            Assert.Equal(appoinment.FinalTime.Second, aggr.Appointment.First().FinalTime.Second);
            Assert.Equal(appoinment.MachineId, aggr.Appointment.First().MachineId);
            Assert.Equal(appoinment.MediaId, aggr.Appointment.First().MediaId);
            Assert.Equal(appoinment.PartnerId, aggr.Appointment.First().PartnerId);
            Assert.Equal(appoinment.PatientId, aggr.Appointment.First().PatientId);
            Assert.Equal(appoinment.PayableId, aggr.Appointment.First().PayableId);
            Assert.Equal(appoinment.ServiceId, aggr.Appointment.First().ServiceId);
            Assert.Equal(appoinment.ServiceLevelId, aggr.Appointment.First().ServiceLevelId);
            Assert.Equal(appoinment.ServiceTypeId, aggr.Appointment.First().ServiceTypeId);
            Assert.Equal(appoinment.SessionId, aggr.Appointment.First().SessionId);
            Assert.Equal(appoinment.StatusType, aggr.Appointment.First().StatusType);
            Assert.Equal(appoinment.TimeZoneId, aggr.Appointment.First().TimeZoneId);
            Assert.Equal(appoinment.TimeStamp.Second, aggr.Appointment.First().TimeStamp.Second);

            var sessionDevice = session.SessionDevice.First();
            Assert.Equal(sessionDevice.Id, aggr.SessionDevice.First().Id);
            Assert.Equal(sessionDevice.DeviceGroup, aggr.SessionDevice.First().DeviceGroup);
            Assert.Equal(sessionDevice.DeviceId, aggr.SessionDevice.First().DeviceId);
            Assert.Equal(sessionDevice.SapCode, aggr.SessionDevice.First().SapCode);
            Assert.Equal(sessionDevice.SerialNumber, aggr.SessionDevice.First().SerialNumber);
            Assert.Equal(sessionDevice.SessionId, aggr.SessionDevice.First().SessionId);
            Assert.Equal(sessionDevice.TimeStamp.Second, aggr.SessionDevice.First().TimeStamp.Second);

            var diagnosis = session.Diagnosis.First();
            Assert.Equal(diagnosis.Id, aggr.Diagnosis.First().Id);
            Assert.Equal(diagnosis.Appraisal, aggr.Diagnosis.First().Appraisal);
            Assert.Equal(diagnosis.Name, aggr.Diagnosis.First().Name);
            Assert.Equal(diagnosis.SessionId, aggr.Diagnosis.First().SessionId);
            Assert.Equal(diagnosis.TimeStamp.Second, aggr.Diagnosis.First().TimeStamp.Second);
        }

        SessionAggregate GenerateRandomAggregate()
        {
            var sessionGraph = new SessionAggregate
            {
                Id = Guid.NewGuid(),
                EndTime = DateTimeOffset.Now.AddDays(-1),
                InitialTime = DateTimeOffset.Now.AddDays(-2),
                InvoiceId = Guid.NewGuid(),
                InterpretationDone = new Random().Next(0, 100) > 50,
                SignedDate = DateTimeOffset.Now,
                GuardianId = Guid.NewGuid(),
                PartnerId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PayableId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                ServiceLevelId = Guid.NewGuid(),
                MediaId = Guid.NewGuid(),
                MachineId = Guid.NewGuid(),
                CommissionId = Guid.NewGuid()
            };

            sessionGraph.Appointment = new List<Appointment>
            {
                new Appointment
                {
                    Id =  Guid.NewGuid(),
                    InitialTime = DateTime.Now.AddDays(-2),
                    FinalTime = DateTime.Now.AddDays(-1),
                    MachineId =  Guid.NewGuid(),
                    MediaId =  Guid.NewGuid(),
                    PartnerId =  Guid.NewGuid(),
                    PatientId =  Guid.NewGuid(),
                    PayableId =  Guid.NewGuid(),
                    ServiceId =  Guid.NewGuid(),
                    ServiceLevelId =  Guid.NewGuid(),
                    ServiceTypeId =  Guid.NewGuid(),
                    SessionId = sessionGraph.Id,
                    StatusType = new Random().Next(),
                    TimeStamp = DateTimeOffset.Now,
                    TimeZoneId = Guid.NewGuid()
                }
            };

            sessionGraph.SessionDevice = new List<SessionDevice>
            {
                new SessionDevice
                {
                    Id = Guid.NewGuid(),
                    SerialNumber = StringExtension.RandomString(17),
                    SapCode =  StringExtension.RandomString(20),
                    DeviceGroup =  StringExtension.RandomString(10),
                    DeviceId = Guid.NewGuid(),
                    SessionId = sessionGraph.Id
                }
            };

            sessionGraph.Diagnosis = new List<Diagnosis>
            {
                new Diagnosis
                {
                    Id = Guid.NewGuid(),
                    Name = StringExtension.RandomString(10),
                    Appraisal = StringExtension.RandomString(10),
                    TimeStamp =  DateTimeOffset.Now,
                    SessionId = sessionGraph.Id
                }
            };

            return sessionGraph;
        }
        BaseEvent GenerateMessage(SessionAggregate aggregate)
        {
            
            var serializedAggregate = new JsonSerializer().Serialize<SessionAggregate>(aggregate);
            return new BaseEvent
            {
                EventID = Guid.NewGuid(),
                MessageOriginator = "Tester",
                MessageType = typeof(FinisedEvaSession).Name,
                Topic = "Session",
                Aggregate = new CryptoManager().Encrypt(serializedAggregate, HostPasswordConfigFake.GetHostPassword())
            };
        }        
    }


}
