using Davalor.SAP.Messages.FreeSessionReason;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class FreeSessionReasonRepository : BaseRepository<FreeSessionReasonAggregate>
    {
        public FreeSessionReasonRepository(string DbConnectionString) : base(DbConnectionString) { }
        public FreeSessionReasonAggregate Get(Guid id)
        {
            var query = "Select [Id],[NameKeyId],[SapCode],[TimeStamp] from [VisionLocalIntegrationTests].[dbo].[FreeSessionReason] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new FreeSessionReasonAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            SapCode = r.GetString(2),
                            TimeStamp = r.GetDateTimeOffset(3)
                        };
                });
        }

        public void Insert(FreeSessionReasonAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[FreeSessionReason]([Id],[NameKeyId],[SapCode],[TimeStamp]) VALUES (@id, @NameKeyId, @SapCode, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
