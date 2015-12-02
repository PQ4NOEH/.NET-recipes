using Davalor.SAP.Messages.Currency;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class CurrencyRepository : BaseRepository<CurrencyAggregate>
    {
        public CurrencyRepository(string DbConnectionString) : base(DbConnectionString) { }
        public CurrencyAggregate Get(Guid id)
        {
            var query = "Select Id, IsoCodeChar, IsoCodeNum, SapCode, Decimals, TimeStamp from [VisionLocalIntegrationTests].[dbo].[Currency] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new CurrencyAggregate
                        {
                            Id = r.GetGuid(0),
                            IsoCodeChar = r.GetString(1),
                            IsoCodeNum = r.GetString(2),
                            SapCode = r.GetString(3),
                            Decimals = r.GetInt32(4),
                            TimeStamp = r.GetDateTimeOffset(5)
                        };
                });
        }

        public void Insert(CurrencyAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Currency]([Id],[IsoCodeChar],[IsoCodeNum],[SapCode], [Decimals],[TimeStamp]) VALUES (@id, @IsoCodeChar, @IsoCodeNum, @SapCode, @Decimals, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@IsoCodeChar", aggregate.IsoCodeChar));
                    cmd.Parameters.Add(new SqlParameter("@IsoCodeNum", aggregate.IsoCodeNum));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@Decimals", aggregate.Decimals));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
