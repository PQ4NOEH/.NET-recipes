using Davalor.VisionLocal.Messages.Session;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class SessionRepository: BaseRepository<SessionAggregate>
    {
        public SessionRepository(string DbConnectionString) : base(DbConnectionString) { }
        public SessionAggregate Get(Guid id)
        {
            var query = "SELECT [Id] ,[InitialTime] ,[EndTime],[InvoiceId],[InvoiceRequired],[InterpretationDone],[SignedDate],"+
                "[GuardianId],[PartnerId],[PatientId],[ServiceLevelId],[ServiceId],[MediaId],[MachineId],[PayableId],[CommissionId],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[Session] " +
                " where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new SessionAggregate
                        {
                            Id = r.GetGuid(0),
                            InitialTime = r.GetDateTimeOffset(1),
                            EndTime = r.GetDateTimeOffset(2),
                            InvoiceId = r.GetGuid(3),
                            InvoiceRequired = r.GetInt32(4),
                            InterpretationDone = r.GetBoolean(5),
                            SignedDate = r.GetDateTimeOffset(6),
                            GuardianId = r.GetGuid(7),
                            PartnerId = r.GetGuid(8),
                            PatientId = r.GetGuid(9),
                            ServiceLevelId = r.GetGuid(10),
                            ServiceId = r.GetGuid(11),
                            MediaId = r.GetGuid(12),
                            MachineId = r.GetGuid(13),
                            PayableId = r.GetGuid(14),
                            CommissionId = r.GetGuid(15),
                            TimeStamp = r.GetDateTimeOffset(16),
                        };
                });

            query = "SELECT [Id],[InitialTime],[FinalTime],[StatusType],[TimeZoneId],[ServiceTypeId],[ServiceLevelId],[ServiceId],[MediaId]" +
                ",[PatientId],[MachineId],[PayableId],[SessionId],[PartnerId],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[Appointment] WHERE SESSIONID = '" + id.ToString() + "'";
            resultGraph.Appointment = this.Query(query, r =>
                {
                    return new SessionAggregate
                        {
                            Appointment = new List<Appointment>
                            { 
                                new Appointment{
                                    Id = r.GetGuid(0),
                                    InitialTime = r.GetDateTime(1),
                                    FinalTime = r.GetDateTime(2),
                                    StatusType = r.GetInt32(3),
                                    TimeZoneId = r.GetGuid(4),
                                    ServiceTypeId = r.GetGuid(5),
                                    ServiceLevelId = r.GetGuid(6),
                                    ServiceId = r.GetGuid(7),
                                    MediaId= r.GetGuid(8),
                                    PatientId = r.GetGuid(9),
                                    MachineId = r.GetGuid(10),
                                    PayableId = r.GetGuid(11),
                                    SessionId = r.GetGuid(12),
                                    PartnerId = r.GetGuid(13),
                                    TimeStamp = r.GetDateTimeOffset(14)
                                }
                            }
                        };
                }).Appointment;
            query = "SELECT [Id],[Name],[Appraisal],[SessionId],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[Diagnosis] WHERE SESSIONID = '" + id.ToString() + "'";
            resultGraph.Diagnosis = this.Query(query, r =>
            {
                return new SessionAggregate
                {
                    Diagnosis = new List<Diagnosis>
                            { 
                                new Diagnosis{
                                    Id = r.GetGuid(0),
                                    Name = r.GetString(1),
                                    Appraisal = r.GetString(2),
                                    SessionId = r.GetGuid(3),
                                    TimeStamp = r.GetDateTimeOffset(4)
                                }
                            }
                };
            }).Diagnosis;

            query = "SELECT [Id],[SapCode],[SerialNumber],[DeviceGroup],[DeviceId],[SessionId],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[SessionDevice] WHERE SESSIONID = '" + id.ToString() + "'";
            resultGraph.SessionDevice = this.Query(query, r =>
            {
                return new SessionAggregate
                {
                    SessionDevice = new List<SessionDevice>
                            { 
                                new SessionDevice{
                                    Id = r.GetGuid(0),
                                    SapCode = r.GetString(1),
                                    SerialNumber  = r.GetString(2),
                                    DeviceGroup  = r.GetString(3),
                                    DeviceId  = r.GetGuid(4),
                                    SessionId = r.GetGuid(5),
                                    TimeStamp = r.GetDateTimeOffset(6)
                                }
                            }
                };
            }).SessionDevice;
            return resultGraph;
        }
    }
}
