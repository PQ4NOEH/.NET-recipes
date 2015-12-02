using Davalor.SAP.Messages.Country;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class CountryRepository : BaseRepository<CountryAggregate>
    {
        public CountryRepository(string DbConnectionString) : base(DbConnectionString) {}
        public CountryAggregate Get(Guid id)
        {
            var query = "Select Id, NameKeyId, CurrencyId, SapCode, TimeStamp from [VisionLocalIntegrationTests].[dbo].[Country] where id = '" + id.ToString()+"'";
            return this.Query(query, r =>
                {
                    return new CountryAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            CurrencyId = r.GetGuid(2),
                            SapCode = r.GetString(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
        }

        public void Insert (CountryAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Country]([Id],[NameKeyId],[CurrencyId],[SapCode],[TimeStamp]) VALUES (@id, @NameKeyId, @CurrencyId, @SapCode,@TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                    cmd.Parameters.Add(new SqlParameter("@CurrencyId", aggregate.CurrencyId));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
