using Davalor.SAP.Messages.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class ServiceRepository: BaseRepository<ServiceAggregate>
    {
        public ServiceRepository(string DbConnectionString) : base(DbConnectionString) { }
        public ServiceAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[NameKeyId],[LongDescriptionKeyId],[Cover],[CoverType],[Deleted],[ServiceTypeId],[DecisionTreeId],"+
                "[SapCode],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[Service] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new ServiceAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            LongDescriptionKeyId = r.GetString(2),
                            Cover = r.GetValue(3) as byte[],
                            CoverType = r.GetString(4),
                            Deleted = r.GetInt32(5),
                            ServiceTypeId = r.GetGuid(6),
                            DecisionTreeId = r.GetGuid(7),
                            SapCode = r.GetString(8),
                            TimeStamp = r.GetDateTimeOffset(9),
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT s.[Id], s.[NameKeyId], s.[LongDescriptionKeyId], s.[Deleted], s.[ServiceId], s.[SapCode], s.[TimeStamp]," +
                " p.[Id],p.[BeginPeriod], p.[EndPeriod], p.[Price], p.[CurrencyId], p.[ServiceLevelId], p.[CountryId], p.[TaxClass]" +
                ", p.[SapCode], p.[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[ServiceLevel] s inner join [VisionLocalIntegrationTests].[dbo].[ServicePrice] p on s.id = p.ServiceLevelId " + 
                " WHERE ServiceId = '" + id.ToString() + "'";
            resultGraph.ServiceLevel = this.Query(query, r =>
                {
                    return new ServiceAggregate
                        {
                            ServiceLevel = new List<ServiceLevel>
                            { 
                                new ServiceLevel{
                                    Id = r.GetGuid(0),
                                    NameKeyId = r.GetString(1),
                                    LongDescriptionKeyId = r.GetString(2),
                                    Deleted = r.GetInt32(3),
                                    ServiceId = r.GetGuid(4),
                                    SapCode = r.GetString(5),
                                    TimeStamp = r.GetDateTimeOffset(6),
                                    ServicePrice = new List<ServicePrice>
                                    {
                                        new ServicePrice
                                        {
                                            Id = r.GetGuid(7),
                                            BeginPeriod = r.GetDateTime(8),
                                            EndPeriod = r.GetDateTime(9),
                                            Price = (float)r.GetValue(10),
                                            CurrencyId = r.GetGuid(11),
                                            ServiceLevelId = r.GetGuid(12),
                                            CountryId = r.GetGuid(13),
                                            TaxClass = r.GetString(14),
                                            SapCode = r.GetString(15),
                                            TimeStamp = r.GetDateTimeOffset(16),
                                        }
                                    }
                                }
                            }
                        };
                }).ServiceLevel;
            query = "SELECT [Id],[NameKeyId],[LongDescriptionKeyId],[Length],[Type],[Deleted],[SapCode],[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[ServiceType] "+
                " WHERE id = '" + resultGraph.ServiceTypeId.ToString() + "'";
            resultGraph.ServiceType = this.Query(query, r =>
            {
                return new ServiceAggregate
                {
                    ServiceType = new ServiceType{
                        Id = r.GetGuid(0),
                        NameKeyId = r.GetString(1),
                        LongDescriptionKeyId = r.GetString(2),
                        Length = r.GetInt32(3),
                        Type = r.GetInt32(4),
                        Deleted = r.GetInt32(5),
                        SapCode = r.GetString(6),
                        TimeStamp = r.GetDateTimeOffset(7)
                    }
                };
            }).ServiceType;

            return resultGraph;
        }

        public void Insert(ServiceAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[ServiceType]( [Id],[NameKeyId],[LongDescriptionKeyId],[Length],[Type],[Deleted],[SapCode],[TimeStamp])" +
                            " VALUES (@Id, @NameKeyId, @LongDescriptionKeyId, @Length, @Type, @Deleted, @SapCode, @TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@id", aggregate.ServiceType.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.ServiceType.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@LongDescriptionKeyId", aggregate.ServiceType.LongDescriptionKeyId));
                        cmd.Parameters.Add(new SqlParameter("@Length", aggregate.ServiceType.Length));
                        cmd.Parameters.Add(new SqlParameter("@Type", aggregate.ServiceType.Type));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.ServiceType.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.ServiceType.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.ServiceType.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Service]( [Id],[NameKeyId],[LongDescriptionKeyId],[Cover],[CoverType],[Deleted],[ServiceTypeId],[DecisionTreeId],[SapCode],[TimeStamp] )" +
                            " VALUES (@Id, @NameKeyId, @LongDescriptionKeyId, @Cover, @CoverType, @Deleted, @ServiceTypeId, @DecisionTreeId, @SapCode, @TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                        cmd.Parameters.Add(new SqlParameter("@LongDescriptionKeyId", aggregate.LongDescriptionKeyId));
                        cmd.Parameters.Add(new SqlParameter("@Cover", aggregate.Cover));
                        cmd.Parameters.Add(new SqlParameter("@CoverType", aggregate.CoverType));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@ServiceTypeId", aggregate.ServiceTypeId));
                        cmd.Parameters.Add(new SqlParameter("@DecisionTreeId", aggregate.DecisionTreeId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var serviceLevel in aggregate.ServiceLevel)
                    {

                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[ServiceLevel]( [Id],[NameKeyId], [LongDescriptionKeyId],[Deleted], [ServiceId], [SapCode],[TimeStamp] )" +
                                " VALUES (@Id, @NameKeyId, @LongDescriptionKeyId, @Deleted, @ServiceId, @SapCode,@TimeStamp )";
                            cmd.Parameters.Add(new SqlParameter("@id", serviceLevel.Id));
                            cmd.Parameters.Add(new SqlParameter("@NameKeyId", serviceLevel.NameKeyId));
                            cmd.Parameters.Add(new SqlParameter("@LongDescriptionKeyId", serviceLevel.LongDescriptionKeyId));
                            cmd.Parameters.Add(new SqlParameter("@Deleted", serviceLevel.Deleted));
                            cmd.Parameters.Add(new SqlParameter("@ServiceId", serviceLevel.ServiceId));
                            cmd.Parameters.Add(new SqlParameter("@SapCode", serviceLevel.SapCode));
                            cmd.Parameters.Add(new SqlParameter("@TimeStamp", serviceLevel.TimeStamp));
                            cmd.ExecuteNonQuery();
                        }
                        if (serviceLevel.ServicePrice.Any())
                        {

                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[ServicePrice]" +
                                    "( [Id], [BeginPeriod], [EndPeriod], [Price], [CurrencyId], [ServiceLevelId], [CountryId], [TaxClass], [SapCode], [TimeStamp] )" +
                                    " VALUES ( @Id, @BeginPeriod, @EndPeriod, @Price, @CurrencyId, @ServiceLevelId, @CountryId, @TaxClass, @SapCode, @TimeStamp )";
                                cmd.Parameters.Add(new SqlParameter("@id", serviceLevel.ServicePrice.First().Id));
                                cmd.Parameters.Add(new SqlParameter("@BeginPeriod", serviceLevel.ServicePrice.First().BeginPeriod));
                                cmd.Parameters.Add(new SqlParameter("@EndPeriod", serviceLevel.ServicePrice.First().EndPeriod));
                                cmd.Parameters.Add(new SqlParameter("@Price", serviceLevel.ServicePrice.First().Price));
                                cmd.Parameters.Add(new SqlParameter("@CurrencyId", serviceLevel.ServicePrice.First().CurrencyId));
                                cmd.Parameters.Add(new SqlParameter("@ServiceLevelId", serviceLevel.ServicePrice.First().ServiceLevelId));
                                cmd.Parameters.Add(new SqlParameter("@CountryId", serviceLevel.ServicePrice.First().CountryId));
                                cmd.Parameters.Add(new SqlParameter("@TaxClass", serviceLevel.ServicePrice.First().TaxClass));
                                cmd.Parameters.Add(new SqlParameter("@SapCode", serviceLevel.ServicePrice.First().SapCode));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", serviceLevel.ServicePrice.First().TimeStamp));
                                cmd.ExecuteNonQuery();
                            }
                        }
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
