using Davalor.SAP.Messages.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class MediaRepository : BaseRepository<MediaAggregate>
    {
        public MediaRepository(string DbConnectionString) : base(DbConnectionString) { }
        public MediaAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[ShortName],[NameKeyId],[LongDescriptionKeyId],[Cover],[CoverType],[Trailer],[TrailerType],[Deleted],"+
                "[NeedsInitialization],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Media] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new MediaAggregate
                        {
                            Id = r.GetGuid(0),
                            ShortName = r.GetString(1),
                            NameKeyId = r.GetString(2),
                            LongDescriptionKeyId = r.GetString(3),
                            Cover = r.GetValue(4) as byte[],
                            CoverType = r.GetString(5),
                            Trailer = r.GetValue(6) as byte[],
                            TrailerType = r.GetString(7),
                            Deleted = r.GetInt32(8),
                            NeedsInitialization = r.GetBoolean(9),
                            SapCode = r.GetString(10),
                            TimeStamp = r.GetDateTimeOffset(11)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id],[DeviceGroupId],[Deleted],[MediaId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[MediaDeviceGroup] "+
                "where MediaId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.MediaDeviceGroup = this.Query(query, r =>
                {
                    return new MediaAggregate
                        {
                            MediaDeviceGroup = new List<MediaDeviceGroup>
                            {
                                new MediaDeviceGroup
                                {
                                    Id = r.GetGuid(0),
                                    DeviceGroupId = r.GetGuid(1),
                                    Deleted = r.GetInt32(2),
                                    MediaId = r.GetGuid(3),
                                    TimeStamp = r.GetDateTimeOffset(4)
                                }
                            }
                        };
                }).MediaDeviceGroup;
            query = "SELECT [Id] ,[MachineId] ,[MediaId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[MediaMachine]"+
                " where MediaId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.MediaMachine = this.Query(query, r =>
            {
                return new MediaAggregate
                {
                    MediaMachine = new Collection<MediaMachine>
                    {
                        new MediaMachine
                        {
                            Id = r.GetGuid(0),
                            MachineId = r.GetGuid(1),
                            MediaId = r.GetGuid(2),
                            TimeStamp = r.GetDateTimeOffset(3)
                        }
                    }
                    
                };
            }).MediaMachine;
            query = "SELECT [Id] ,[ServiceLevelId] ,[MediaId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[MediaServiceLevel] where MediaId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.MediaServiceLevel = this.Query(query, r =>
            {
                return new MediaAggregate
                {
                    MediaServiceLevel = new Collection<MediaServiceLevel>
                    {
                        new MediaServiceLevel
                        {
                            Id = r.GetGuid(0),
                            ServiceLevelId = r.GetGuid(1),
                            MediaId = r.GetGuid(2),
                            TimeStamp = r.GetDateTimeOffset(3)
                        }
                    }

                };
            }).MediaServiceLevel;
            return resultGraph;
        }

        public void Insert(MediaAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Media]([Id],[ShortName],[NameKeyId],[LongDescriptionKeyId],[Cover],[CoverType],[Trailer],[TrailerType],[Deleted],[NeedsInitialization],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@ShortName,@NameKeyId,@LongDescriptionKeyId,@Cover,@CoverType,@Trailer,@TrailerType,@Deleted,@NeedsInitialization,@SapCode,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@ShortName", aggregate.ShortName));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@LongDescriptionKeyId", aggregate.LongDescriptionKeyId));
                        cmd.Parameters.Add(new SqlParameter("@Cover", aggregate.Cover));
                        cmd.Parameters.Add(new SqlParameter("@CoverType", aggregate.CoverType));
                        cmd.Parameters.Add(new SqlParameter("@Trailer", aggregate.Trailer));
                        cmd.Parameters.Add(new SqlParameter("@TrailerType", aggregate.TrailerType));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@NeedsInitialization", aggregate.NeedsInitialization));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[MediaDeviceGroup] ([Id],[DeviceGroupId],[Deleted],[MediaId],[TimeStamp])" +
                            " VALUES (@Id,@DeviceGroupId,@Deleted,@MediaId,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.MediaDeviceGroup.First().Id));
                        cmd.Parameters.Add(new SqlParameter("@DeviceGroupId", aggregate.MediaDeviceGroup.First().DeviceGroupId));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.MediaDeviceGroup.First().Deleted));
                        cmd.Parameters.Add(new SqlParameter("@MediaId", aggregate.MediaDeviceGroup.First().MediaId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.MediaDeviceGroup.First().TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[MediaMachine]([Id] ,[MachineId] ,[MediaId],[TimeStamp])" +
                            " VALUES (@Id ,@MachineId ,@MediaId,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.MediaMachine.First().Id));
                        cmd.Parameters.Add(new SqlParameter("@MachineId", aggregate.MediaMachine.First().MachineId));
                        cmd.Parameters.Add(new SqlParameter("@MediaId", aggregate.MediaMachine.First().MediaId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.MediaMachine.First().TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[MediaServiceLevel]([Id] ,[ServiceLevelId] ,[MediaId],[TimeStamp])" +
                            " VALUES (@Id ,@ServiceLevelId ,@MediaId,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.MediaServiceLevel.First().Id));
                        cmd.Parameters.Add(new SqlParameter("@ServiceLevelId", aggregate.MediaServiceLevel.First().ServiceLevelId));
                        cmd.Parameters.Add(new SqlParameter("@MediaId", aggregate.MediaServiceLevel.First().MediaId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.MediaServiceLevel.First().TimeStamp));
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
