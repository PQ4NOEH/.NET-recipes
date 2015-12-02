using Davalor.SAP.Messages.Partner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class PartnerRepository : BaseRepository<PartnerAggregate>
    {
        public PartnerRepository(string DbConnectionString) : base(DbConnectionString) { }
        public PartnerAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[Name],[Phone],[Email],[Url],[Deleted],[TimeZoneId],[LanguageId],[LocationId]," + 
                "[PartnerChainId],[CurrencyId],[TaxClass],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Partner]" +
                "where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new PartnerAggregate
                        {
                            Id = r.GetGuid(0),
                            Name = r.GetString(1),
                            Phone = r.GetString(2),
                            Email = r.GetString(3),
                            Url = r.GetString(4),
                            Deleted = r.GetInt32(5),
                            TimeZoneId = r.GetGuid(6),
                            LanguageId = r.GetGuid(7),
                            LocationId = r.GetGuid(8),
                            PartnerChainId = r.GetGuid(9),
                            CurrencyId = r.GetGuid(10),
                            TaxClass = r.GetString(11),
                            SapCode = r.GetString(12),
                            TimeStamp = r.GetDateTimeOffset(13)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id],[Name],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[PartnerChain] " +
                "where Id = '" + resultGraph.PartnerChainId.ToString() + "'";
            resultGraph.PartnerChain = this.Query(query, r =>
                {
                    return new PartnerAggregate
                        {
                            PartnerChain = new PartnerChain
                            {
                                Id = r.GetGuid(0),
                                Name = r.GetString(1),
                                SapCode = r.GetString(2),
                                TimeStamp = r.GetDateTimeOffset(3)
                            }
                        };
                }).PartnerChain;
            //query = "SELECT [Id],[City],[Street],[PostalCode],[Longitude],[Latitude],[CountryId],[RegionId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Location]" +
            //    " where Id = '" + resultGraph.LocationId.ToString() + "'";
            //resultGraph.Location = this.Query(query, r =>
            //{
            //    return new PartnerAggregate
            //    {
            //        Location = new Location
            //        {
            //            Id = r.GetGuid(0),
            //            City = r.GetString(1),
            //            Street = r.GetString(2),
            //            PostalCode = r.GetString(3),
            //            Longitude = r.GetDouble(4),
            //            Latitude = r.GetDouble(5),
            //            CountryId = r.GetGuid(6),
            //            RegionId = r.GetGuid(7),
            //            TimeStamp = r.GetDateTimeOffset(8)
                       
            //        }
                    
            //    };
            //}).Location;
            
            return resultGraph;
        }

        public void Insert(PartnerAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[PartnerChain] ([Id],[Name],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@Name,@SapCode,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.PartnerChain.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", aggregate.PartnerChain.Name));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.PartnerChain.SapCode));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.PartnerChain.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Partner]([Id],[Name],[Phone],[Email],[Url],[Deleted],[TimeZoneId],[LanguageId],[LocationId],[PartnerChainId],[CurrencyId],[TaxClass],[SapCode],[TimeStamp])" +
                            " VALUES (@Id ,@Name ,@Phone ,@Email ,@Url ,@Deleted ,@TimeZoneId ,@LanguageId ,@LocationId ,@PartnerChainId ,@CurrencyId ,@TaxClass ,@SapCode ,@TimeStamp )";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", aggregate.Name));
                        cmd.Parameters.Add(new SqlParameter("@Phone", aggregate.Phone));
                        cmd.Parameters.Add(new SqlParameter("@Email", aggregate.Email));
                        cmd.Parameters.Add(new SqlParameter("@Url", aggregate.Url));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@TimeZoneId", aggregate.TimeZoneId));
                        cmd.Parameters.Add(new SqlParameter("@LanguageId", aggregate.LanguageId));
                        cmd.Parameters.Add(new SqlParameter("@LocationId", aggregate.LocationId));
                        cmd.Parameters.Add(new SqlParameter("@PartnerChainId", aggregate.PartnerChainId));
                        cmd.Parameters.Add(new SqlParameter("@CurrencyId", aggregate.CurrencyId));
                        cmd.Parameters.Add(new SqlParameter("@TaxClass", aggregate.TaxClass));
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
