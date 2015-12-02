using Davalor.PortalPaciente.Messages.Disclaimer;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class DisclaimerRepository : BaseRepository<DisclaimerAggregate>
    {
        public DisclaimerRepository(string DbConnectionString) : base(DbConnectionString) { }
        public DisclaimerAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[Version],[Code],[LinkKeyId],[TextKeyId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Disclaimer] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new DisclaimerAggregate
                        {
                            Id = r.GetGuid(0),
                            Version = r.GetInt32(1),
                            Code = r.GetString(2),
                            LinkKeyId = r.GetString(3),
                            TextKeyId = r.GetString(4),
                            TimeStamp = r.GetDateTimeOffset(5)
                        };
                });
        }

        public void Insert(DisclaimerAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Disclaimer]([Id],[Version],[Code],[LinkKeyId],[TextKeyId],[TimeStamp]) VALUES (@id, @Version, @Code, @LinkKeyId, @TextKeyId, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@Version", aggregate.Version));
                    cmd.Parameters.Add(new SqlParameter("@Code", aggregate.Code));
                    cmd.Parameters.Add(new SqlParameter("@LinkKeyId", aggregate.LinkKeyId));
                    cmd.Parameters.Add(new SqlParameter("@TextKeyId", aggregate.TextKeyId));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
