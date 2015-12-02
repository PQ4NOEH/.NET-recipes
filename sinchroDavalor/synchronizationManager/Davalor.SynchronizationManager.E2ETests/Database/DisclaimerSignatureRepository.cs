using Davalor.PortalPaciente.Messages.DisclaimerSignature;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class DisclaimerSignatureRepository : BaseRepository<DisclaimerSignatureAggregate>
    {
        public DisclaimerSignatureRepository(string DbConnectionString) : base(DbConnectionString) { }
        public DisclaimerSignatureAggregate Get(Guid id)
        {
            var query = "SELECT D.[Id],[State],[SignedDate],[SignatureId],[PatientId],[DisclaimerId],D.[TimeStamp], s.id, s.SafekeepingIdentifier, S.TimeStamp" +
                " FROM [VisionLocalIntegrationTests].[dbo].[DavalorDisclaimer] D inner JOIN [VisionLocalIntegrationTests].[dbo].[Signature] S ON D.SignatureId = S.Id" +
                " where D.id = '" + id.ToString() + "'";
           return this.Query(query, r =>
                {
                    return new DisclaimerSignatureAggregate
                        {
                            Id = r.GetGuid(0),
                            State = r.GetInt32(1),
                            SignedDate = r.GetDateTimeOffset(2),
                            SignatureId = r.GetGuid(3),
                            PatientId = r.GetGuid(4),
                            DisclaimerId = r.GetGuid(5),
                            TimeStamp = r.GetDateTimeOffset(6),
                            Signature = new Signature
                            {
                                Id = r.GetGuid(7),
                                SafekeepingIdentifier = r.GetString(8),
                                TimeStamp = r.GetDateTimeOffset(9)
                            }
                        };
                });
        }

        public void Insert(DisclaimerSignatureAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Signature]( [Id],[SafekeepingIdentifier],[TimeStamp])" +
                            " VALUES (@Id,@SafekeepingIdentifier,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Signature.Id));
                        cmd.Parameters.Add(new SqlParameter("@SafekeepingIdentifier", aggregate.Signature.SafekeepingIdentifier));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.Signature.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[DavalorDisclaimer]" +
                            "([Id],[State],[SignedDate],[SignatureId],[PatientId],[DisclaimerId],[TimeStamp])" +
                            " VALUES (@Id,@State, @SignedDate, @SignatureId, @PatientId, @DisclaimerId,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@State", aggregate.State));
                        cmd.Parameters.Add(new SqlParameter("@SignedDate", aggregate.SignedDate));
                        cmd.Parameters.Add(new SqlParameter("@SignatureId", aggregate.SignatureId));
                        cmd.Parameters.Add(new SqlParameter("@PatientId", aggregate.PatientId));
                        cmd.Parameters.Add(new SqlParameter("@DisclaimerId", aggregate.DisclaimerId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
