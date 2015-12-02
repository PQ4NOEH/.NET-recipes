using Davalor.SAP.Messages.Printer;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class PrinterRepository : BaseRepository<PrinterAggregate>
    {
        public PrinterRepository(string DbConnectionString) : base(DbConnectionString) { }
        public PrinterAggregate Get(Guid id)
        {
            var query = "Select [Id],[Code],[PartnerId], [POSPrint], [TimeStamp] from [VisionLocalIntegrationTests].[dbo].[Printer] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new PrinterAggregate
                        {
                            Id = r.GetGuid(0),
                            Code = r.GetString(1),
                            PartnerId = r.GetGuid(2),
                            POSPrint = r.GetBoolean(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
        }

        public void Insert(PrinterAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Printer]( [Id],[Code],[PartnerId], [POSPrint], [TimeStamp]) VALUES (@id, @Code, @PartnerId, @POSPrint, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@Code", aggregate.Code));
                    cmd.Parameters.Add(new SqlParameter("@PartnerId", aggregate.PartnerId));
                    cmd.Parameters.Add(new SqlParameter("@POSPrint", aggregate.POSPrint));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
