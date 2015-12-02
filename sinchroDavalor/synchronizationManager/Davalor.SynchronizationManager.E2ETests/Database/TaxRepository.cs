using Davalor.SAP.Messages.Tax;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class TaxRepository : BaseRepository<TaxAggregate>
    {
        public TaxRepository(string DbConnectionString) : base(DbConnectionString) { }
        public TaxAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[CountryId],[CurrencyId],[TaxClassPartner] ,[TaxClassServicePrice],[BeginPeriod] ,[EndPeriod] ," + 
                " [Rule],[Amount] ,[BaseAmount] ,[NameKeyId] ,[SapCode] ,[TimeStamp]  FROM [VisionLocalIntegrationTests].[dbo].[Tax] "+
                " where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new TaxAggregate
                        {
                            Id = r.GetGuid(0),
                            CountryId = r.GetGuid(1),
                            CurrencyId = r.GetGuid(2),
                            TaxClassPartner = r.GetString(3),
                            TaxClassServicePrice = r.GetString(4),
                            BeginPeriod = r.GetDateTimeOffset(5),
                            EndPeriod = r.GetDateTimeOffset(6),
                            Rule = r.GetString(7),
                            Amount = r.GetDecimal(8),
                            BaseAmount = r.GetDecimal(9),
                            NameKeyId = r.GetString(10),
                            SapCode = r.GetString(11),
                            TimeStamp = r.GetDateTimeOffset(12)
                        };
                });
        }

        public void Insert(TaxAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Tax]( [Id],[CountryId] ,[CurrencyId],[TaxClassPartner]" +
                        " ,[TaxClassServicePrice],[BeginPeriod] ,[EndPeriod] ,[Rule],[Amount] ,[BaseAmount] ,[NameKeyId] ,[SapCode] ,[TimeStamp])" +
                        " VALUES (@Id,@CountryId ,@CurrencyId,@TaxClassPartner,@TaxClassServicePrice,@BeginPeriod ,@EndPeriod ,@Rule,@Amount ,@BaseAmount ,@NameKeyId ,@SapCode ,@TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@CountryId", aggregate.CountryId));
                    cmd.Parameters.Add(new SqlParameter("@CurrencyId", aggregate.CurrencyId));
                    cmd.Parameters.Add(new SqlParameter("@TaxClassPartner", aggregate.TaxClassPartner));
                    cmd.Parameters.Add(new SqlParameter("@TaxClassServicePrice", aggregate.TaxClassServicePrice));
                    cmd.Parameters.Add(new SqlParameter("@BeginPeriod", aggregate.BeginPeriod));
                    cmd.Parameters.Add(new SqlParameter("@EndPeriod", aggregate.EndPeriod));
                    cmd.Parameters.Add(new SqlParameter("@Rule", aggregate.Rule));
                    cmd.Parameters.Add(new SqlParameter("@Amount", aggregate.Amount));
                    cmd.Parameters.Add(new SqlParameter("@BaseAmount", aggregate.BaseAmount));
                    cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
