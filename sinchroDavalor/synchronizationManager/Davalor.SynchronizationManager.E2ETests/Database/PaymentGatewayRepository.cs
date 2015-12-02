using Davalor.SAP.Messages.PaymentGateway;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class PaymentGatewayRepository : BaseRepository<GatewayAggregate>
    {
        public PaymentGatewayRepository(string DbConnectionString) : base(DbConnectionString) { }
        public GatewayAggregate Get(Guid id)
        {
            var query = "Select [Id] ,[GatewayType] ,[GatewayPlatformType] ,[GatewayTypeName] ,[GatewayDescription] ,[TimeStamp]" +
                " from [VisionLocalIntegrationTests].[dbo].[Gateway] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new GatewayAggregate
                        {
                            Id = r.GetGuid(0),
                            GatewayType = r.GetInt32(1),
                            GatewayPlatformType = r.GetInt32(2),
                            GatewayTypeName = r.GetString(3),
                            GatewayDescription = r.GetString(4),
                            TimeStamp = r.GetDateTimeOffset(5)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id], [CountryId], [GatewayId], [TimeStamp]" +
                "  FROM [VisionLocalIntegrationTests].[dbo].[GatewayByCountry] where GatewayId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.GatewayByCountry = this.Query(query, r =>
            {
                return new GatewayAggregate
                {
                    GatewayByCountry = new Collection<GatewayByCountry>
                    {
                        new GatewayByCountry
                        {
                            Id = r.GetGuid(0),
                            CountryId = r.GetGuid(1),
                            GatewayId = r.GetGuid(2),
                            TimeStamp = r.GetDateTimeOffset(3)
                        }
                    }
                };
            }).GatewayByCountry;
            return resultGraph;
        }

        public void Insert(GatewayAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Gateway]" +
                            "([Id] ,[GatewayType] ,[GatewayPlatformType] ,[GatewayTypeName] ,[GatewayDescription] ,[TimeStamp])" +
                            " VALUES (@Id ,@GatewayType ,@GatewayPlatformType ,@GatewayTypeName ,@GatewayDescription ,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@GatewayType", aggregate.GatewayType));
                        cmd.Parameters.Add(new SqlParameter("@GatewayPlatformType", aggregate.GatewayPlatformType));
                        cmd.Parameters.Add(new SqlParameter("@GatewayTypeName", aggregate.GatewayTypeName)); 
                        cmd.Parameters.Add(new SqlParameter("@GatewayDescription", aggregate.GatewayDescription));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    aggregate.GatewayByCountry.ToList().ForEach(aggr =>
                        {
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[GatewayByCountry]"+
                                    " ([Id], [CountryId], [GatewayId], [TimeStamp])" +
                                    " VALUES (@Id, @CountryId, @GatewayId, @TimeStamp)";
                                cmd.Parameters.Add(new SqlParameter("@Id", aggr.Id));
                                cmd.Parameters.Add(new SqlParameter("@CountryId", aggr.CountryId));
                                cmd.Parameters.Add(new SqlParameter("@GatewayId", aggr.GatewayId));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggr.TimeStamp));
                                cmd.ExecuteNonQuery();
                            }
                        });
                   
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
