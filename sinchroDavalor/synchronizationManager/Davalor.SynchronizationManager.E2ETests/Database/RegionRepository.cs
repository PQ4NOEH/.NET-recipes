using Davalor.SAP.Messages.Region;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class RegionRepository : BaseRepository<RegionAggregate>
    {
        public RegionRepository(string DbConnectionString) : base(DbConnectionString) { }
        public RegionAggregate Get(Guid id)
        {
            var query = "Select [Id],[NameKeyId],[CountryId], [SapCode], [TimeStamp] from [VisionLocalIntegrationTests].[dbo].[Region] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new RegionAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            CountryId = r.GetGuid(2),
                            SapCode = r.GetString(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
        }

        public void Insert(RegionAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Region]( [Id],[NameKeyId],[CountryId], [SapCode], [TimeStamp]) VALUES (@id, @NameKeyId, @CountryId, @SapCode, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                    cmd.Parameters.Add(new SqlParameter("@CountryId", aggregate.CountryId));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
