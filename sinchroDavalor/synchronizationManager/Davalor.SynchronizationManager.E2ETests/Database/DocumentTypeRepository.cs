using Davalor.SAP.Messages.DocumentType;
using System;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class DocumentTypeRepository : BaseRepository<DocumentTypeAggregate>
    {
        public DocumentTypeRepository(string DbConnectionString) : base(DbConnectionString) { }
        public DocumentTypeAggregate Get(Guid id)
        {
            var query = "Select [Id],[NameKeyId],[CountryId],[SapCode],[TimeStamp] from [VisionLocalIntegrationTests].[dbo].[DocumentType] where id = '" + id.ToString() + "'";
            return this.Query(query, r =>
                {
                    return new DocumentTypeAggregate
                        {
                            Id = r.GetGuid(0),
                            NameKeyId = r.GetString(1),
                            CountryId = r.GetGuid(2),
                            SapCode = r.GetString(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
        }

        public void Insert(DocumentTypeAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[DocumentType]([Id],[NameKeyId],[CountryId],[SapCode],[TimeStamp]) VALUES (@id, @NameKeyId, @CountryId, @SapCode, @TimeStamp)";
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
