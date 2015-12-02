using Davalor.SAP.Messages.Device;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class DeviceRepository : BaseRepository<DeviceAggregate>
    {
        public DeviceRepository(string DbConnectionString) : base(DbConnectionString) { }
        public DeviceAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[DescriptionKeyId],[SerialNumber],[Deleted],[DeviceGroupId],[MachineId],[SapCode],[TimeStamp]" +
                " FROM [VisionLocalIntegrationTests].[dbo].[Device] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new DeviceAggregate
                        {
                            Id = r.GetGuid(0),
                            DescriptionKeyId = r.GetString(1),
                            SerialNumber = r.GetString(2),
                            Deleted = r.GetInt32(3),
                            DeviceGroupId = r.GetGuid(4),
                            MachineId = r.GetGuid(5),
                            SapCode = r.GetString(6),
                            TimeStamp = r.GetDateTimeOffset(7),
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT DG.[Id],DG.[NameKeyId],DG.[DeviceTypeId],DG.[SapCode],DG.[TimeStamp],DT.[Id],DT.[NameKeyId],DT.[SapCode],DT.[TimeStamp]" +
                "  FROM [VisionLocalIntegrationTests].[dbo].[DeviceGroup] DG inner join [VisionLocalIntegrationTests].[dbo].[DeviceType] DT on DG.DeviceTypeId = DT.Id where DG.id = '" + resultGraph.DeviceGroupId.ToString() + "'";
            resultGraph.DeviceGroup = this.Query(query, r =>
                {
                    return new DeviceAggregate
                        {
                            DeviceGroup = new DeviceGroup
                            {
                                Id = r.GetGuid(0),
                                NameKeyId = r.GetString(1),
                                DeviceTypeId = r.GetGuid(2),
                                SapCode = r.GetString(3),
                                TimeStamp = r.GetDateTimeOffset(4),
                                DeviceType = new DeviceType
                                {
                                    Id = r.GetGuid(5),
                                    NameKeyId = r.GetString(6),
                                    SapCode = r.GetString(7),
                                    TimeStamp = r.GetDateTimeOffset(8),
                                }
                            }
                        };
                }).DeviceGroup;
            return resultGraph;
        }

        public void Insert(DeviceAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[DeviceType]( [Id],[NameKeyId],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@NameKeyId,@SapCode,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.DeviceGroup.DeviceType.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.DeviceGroup.DeviceType.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.DeviceGroup.DeviceType.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.DeviceGroup.DeviceType.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[DeviceGroup]( [Id],[NameKeyId],[DeviceTypeId],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@NameKeyId, @DeviceTypeId, @SapCode,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.DeviceGroup.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.DeviceGroup.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@DeviceTypeId", aggregate.DeviceGroup.DeviceTypeId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.DeviceGroup.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.DeviceGroup.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Device]( [Id],[DescriptionKeyId],[SerialNumber],[Deleted],[DeviceGroupId],[MachineId],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@DescriptionKeyId,@SerialNumber,@Deleted,@DeviceGroupId,@MachineId,@SapCode,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@DescriptionKeyId", aggregate.DescriptionKeyId));
                        cmd.Parameters.Add(new SqlParameter("@SerialNumber", aggregate.SerialNumber));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@DeviceGroupId", aggregate.DeviceGroupId));
                        cmd.Parameters.Add(new SqlParameter("@MachineId", aggregate.MachineId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
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
