using Davalor.VisionLocal.Messages.AuditLogon;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class AuditLogonRepository : BaseRepository<AuditLogonAggregate>
    {
        public AuditLogonRepository(string DbConnectionString) : base(DbConnectionString) { }
        public AuditLogonAggregate Get(Guid id)
        {
            var query = "Select [Id],[UserName],[Ip],[Access],[AccessDate], [PartnerId],[TimeStamp] from [VisionLocalIntegrationTests].[dbo].[AuditLogon] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new AuditLogonAggregate
                        {
                            Id = r.GetGuid(0),
                            UserName = r.GetString(1),
                            Ip = r.GetString(2),
                            Access = r.GetString(3),
                            AccessDate = r.GetDateTimeOffset(4),
                            PartnerId = r.GetGuid(5),
                            TimeStamp = r.GetDateTimeOffset(6)
                        };
                });
        }

        public void Insert(AuditLogonAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[AuditLogon]([Id],[UserName],[Ip],[Access],[AccessDate], [PartnerId],[TimeStamp]) " +
                        " VALUES (@id, @UserName, @Ip, @Access, @AccessDate, @PartnerId, @TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@UserName", aggregate.UserName));
                    cmd.Parameters.Add(new SqlParameter("@Ip", aggregate.Ip));
                    cmd.Parameters.Add(new SqlParameter("@Access", aggregate.Access));
                    cmd.Parameters.Add(new SqlParameter("@AccessDate", aggregate.AccessDate));
                    cmd.Parameters.Add(new SqlParameter("@PartnerId", aggregate.PartnerId));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
