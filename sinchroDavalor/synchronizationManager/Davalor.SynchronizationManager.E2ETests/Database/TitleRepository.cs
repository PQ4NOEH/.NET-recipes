using Davalor.SAP.Messages.Title;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class TitleRepository : BaseRepository<TitleAggregate>
    {
        public TitleRepository(string DbConnectionString) : base(DbConnectionString) { }
        public TitleAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[NameKeyId] ,[Deleted],[SapCode] ,[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Title] " +
                " where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new TitleAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            Deleted = r.GetInt32(2),
                            SapCode = r.GetString(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
        }

        public void Insert(TitleAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Title]( [Id],[NameKeyId] ,[Deleted],[SapCode] ,[TimeStamp])" +
                        " VALUES (@Id,@NameKeyId ,@Deleted,@SapCode,@TimeStamp)";
                    cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                    cmd.Parameters.Add(new SqlParameter("@NameKeyId", aggregate.NameKeyId));
                    cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Deleted));
                    cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
                    cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
