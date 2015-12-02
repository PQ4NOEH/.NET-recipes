using Davalor.SAP.Messages.Location;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class LocationRepository : BaseRepository<LocationAggregate>
    {
        public LocationRepository(string DbConnectionString) : base(DbConnectionString) { }
        public LocationAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[City],[Street],[PostalCode],[Longitude],[Latitude],[CountryId],[RegionId],[TimeStamp] "+ 
                " FROM [VisionLocalIntegrationTests].[dbo].[Location] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new LocationAggregate
                        {
                            Id = r.GetGuid(0),
                            City = r.GetString(1),
                            Street = r.GetString(2),
                            PostalCode = r.GetString(3),
                            Longitude = r.GetDouble(4),
                            Latitude = r.GetDouble(5),
                            CountryId = r.GetGuid(6),
                            RegionId = r.GetGuid(7),
                            TimeStamp = r.GetDateTimeOffset(8),
                        };
                });
        }

        public void Insert(LocationAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Location]"+
                            " ([Id],[City],[Street],[PostalCode],[Longitude],[Latitude],[CountryId],[RegionId],[TimeStamp])" +
                            " VALUES (@Id,@City,@Street,@PostalCode,@Longitude,@Latitude,@CountryId,@RegionId,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@City", aggregate.City));
                        cmd.Parameters.Add(new SqlParameter("@Street", aggregate.Street));
                        cmd.Parameters.Add(new SqlParameter("@PostalCode", aggregate.PostalCode));
                        cmd.Parameters.Add(new SqlParameter("@Longitude", aggregate.Longitude));
                        cmd.Parameters.Add(new SqlParameter("@Latitude", aggregate.Latitude));
                        cmd.Parameters.Add(new SqlParameter("@CountryId", aggregate.CountryId));
                        cmd.Parameters.Add(new SqlParameter("@RegionId", aggregate.RegionId));
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
