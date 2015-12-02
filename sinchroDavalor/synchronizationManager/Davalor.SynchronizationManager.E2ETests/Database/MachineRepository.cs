using Davalor.SAP.Messages.Machine;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class MachineRepository : BaseRepository<MachineAggregate>
    {
        public MachineRepository(string DbConnectionString) : base(DbConnectionString) { }
        public MachineAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[SerialNumber],[DeviceIdentifier],[UserFriendlyName],[DescriptionKeyId],[Ip],[Port],[PortState],[StreamingPort],"+
                "[SortOrder],[Deleted],[PartnerId],[MachineGroupId],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Machine] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new MachineAggregate
                        {
                            Id = r.GetGuid(0),
                            SerialNumber = r.GetString(1),
                            DeviceIdentifier = r.GetString(2),
                            UserFriendlyName = r.GetString(3),
                            DescriptionKeyId = r.GetString(4),
                            Ip = r.GetString(5),
                            Port = r.GetInt32(6),
                            PortState = r.GetInt32(7),
                            StreamingPort = r.GetInt32(8),
                            SortOrder = r.GetInt32(9),
                            Deleted = r.GetInt32(10),
                            PartnerId = r.GetGuid(11),
                            MachineGroupId = r.GetGuid(12),
                            SapCode = r.GetString(13),
                            TimeStamp = r.GetDateTimeOffset(14)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id],[NameKeyId],[Deleted],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[MachineGroup] where id = '" + resultGraph.MachineGroupId.ToString() + "'";
            resultGraph.MachineGroup = this.Query(query, r =>
                {
                    return new MachineAggregate
                        {
                            MachineGroup = new MachineGroup
                            {
                                Id = r.GetGuid(0),
                                NameKeyId = r.GetString(1),
                                Deleted = r.GetInt32(2),
                                SapCode = r.GetString(3),
                                TimeStamp = r.GetDateTimeOffset(4)
                            }
                        };
                }).MachineGroup;
            query = "SELECT [Id],[MachineId],[PrinterId],[Function],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[MachinePrinter] where MachineId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.MachinePrinter = this.Query(query, r =>
            {
                return new MachineAggregate
                {
                    MachinePrinter = new Collection<MachinePrinter>
                    {
                        new MachinePrinter
                        {
                            Id = r.GetGuid(0),
                            MachineId = r.GetGuid(1),
                            PrinterId = r.GetGuid(2),
                            Function = r.GetInt32(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        }
                    }
                    
                };
            }).MachinePrinter;
            return resultGraph;
        }

        public void Insert(MachineAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[MachineGroup]([Id],[NameKeyId],[Deleted],[SapCode],[TimeStamp])" +
                            " VALUES (@Id, @NameKeyId, @Deleted, @SapCode, @TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.MachineGroup.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.MachineGroup.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.MachineGroup.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.MachineGroup.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.MachineGroup.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Machine]" +
                            "([Id],[SerialNumber],[DeviceIdentifier],[UserFriendlyName],[DescriptionKeyId],[Ip],[Port],[PortState],[StreamingPort],[SortOrder],[Deleted],[PartnerId],[MachineGroupId],[SapCode],[TimeStamp])" +
                            " VALUES (@Id, @SerialNumber, @DeviceIdentifier, @UserFriendlyName, @DescriptionKeyId, @Ip, @Port, @PortState, @StreamingPort, @SortOrder, @Deleted, @PartnerId, @MachineGroupId, @SapCode, @TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@SerialNumber", aggregate.SerialNumber));
                        cmd.Parameters.Add(new SqlParameter("@DeviceIdentifier", aggregate.DeviceIdentifier));
                        cmd.Parameters.Add(new SqlParameter("@UserFriendlyName", aggregate.UserFriendlyName));
                        cmd.Parameters.Add(new SqlParameter("@DescriptionKeyId", aggregate.DescriptionKeyId));
                        cmd.Parameters.Add(new SqlParameter("@Ip", aggregate.Ip));
                        cmd.Parameters.Add(new SqlParameter("@Port", aggregate.Port));
                        cmd.Parameters.Add(new SqlParameter("@PortState", aggregate.PortState));
                        cmd.Parameters.Add(new SqlParameter("@StreamingPort", aggregate.StreamingPort));
                        cmd.Parameters.Add(new SqlParameter("@SortOrder", aggregate.SortOrder));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@PartnerId", aggregate.PartnerId));
                        cmd.Parameters.Add(new SqlParameter("@MachineGroupId", aggregate.MachineGroupId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[MachinePrinter]([Id],[MachineId],[PrinterId],[Function],[TimeStamp])" +
                            " VALUES (@Id, @MachineId, @PrinterId, @Function, @TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.MachinePrinter.First().Id));
                        cmd.Parameters.Add(new SqlParameter("@MachineId", aggregate.MachinePrinter.First().MachineId));
                        cmd.Parameters.Add(new SqlParameter("@PrinterId", aggregate.MachinePrinter.First().PrinterId));
                        cmd.Parameters.Add(new SqlParameter("@Function", aggregate.MachinePrinter.First().Function));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.MachinePrinter.First().TimeStamp));
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
