using Davalor.PortalPaciente.Messages.Answer;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class AnswerRepository : BaseRepository<AnswerAggregate>
    {
        public AnswerRepository(string DbConnectionString) : base(DbConnectionString) { }
        public AnswerAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[PersonId],[SessionId],[AppointmentId],[Type],[QuestionId],[TimeStamp]" +
                " FROM [VisionLocalIntegrationTests].[dbo].[Answer]where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new AnswerAggregate
                        {
                            Id = r.GetGuid(0),
                            PersonId = r.GetGuid(1),
                            SessionId = r.GetGuid(2),
                            AppointmentId = r.GetGuid(3),
                            Type = r.GetInt32(4),
                            QuestionId = r.GetGuid(5),
                            TimeStamp = r.GetDateTimeOffset(6)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id],[Type],[ValueNumber],[ValueDecimal],[ValueBoolean],[AnswerId],[ValueCatalogItemId],[TimeStamp] "+
                "FROM [VisionLocalIntegrationTests].[dbo].[AnswerValues] where AnswerId= '" + resultGraph.Id.ToString() + "'";
            resultGraph.AnswerValues = this.Query(query, r =>
            {
                return new AnswerAggregate
                {
                    AnswerValues = new Collection<AnswerValues>
                    {
                        new AnswerValues
                        {
                            Id = r.GetGuid(0),
                            Type = r.GetInt32(1),
                            ValueNumber = r.GetString(2),
                            ValueDecimal = r.GetString(3),
                            ValueBoolean = r.GetString(4),
                            AnswerId = r.GetGuid(5),
                            ValueCatalogItemId =  r.GetGuid(6),
                            TimeStamp = r.GetDateTimeOffset(7)
                        }
                    }
                };
            }).AnswerValues;
            return resultGraph;
        }

        public void Insert(AnswerAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Answer]" +
                            "([Id],[PersonId],[SessionId],[AppointmentId],[Type],[QuestionId],[TimeStamp])" +
                            " VALUES (@Id,@PersonId,@SessionId,@AppointmentId,@Type,@QuestionId,@TimeStamp)";

                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@PersonId", aggregate.PersonId));
                        cmd.Parameters.Add(new SqlParameter("@SessionId", aggregate.SessionId));
                        cmd.Parameters.Add(new SqlParameter("@AppointmentId", aggregate.AppointmentId));
                        cmd.Parameters.Add(new SqlParameter("@Type", aggregate.Type));
                        cmd.Parameters.Add(new SqlParameter("@QuestionId", aggregate.QuestionId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }
                    aggregate.AnswerValues.ToList().ForEach(aggr =>
                        {
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[AnswerValues]" +
                                    " ([Id],[Type],[ValueNumber],[ValueDecimal],[ValueBoolean],[AnswerId],[ValueCatalogItemId],[TimeStamp])" +
                                    " VALUES (@Id,@Type,@ValueNumber,@ValueDecimal,@ValueBoolean,@AnswerId,@ValueCatalogItemId,@TimeStamp)";
                                cmd.Parameters.Add(new SqlParameter("@Id", aggr.Id));
                                cmd.Parameters.Add(new SqlParameter("@Type", aggr.Type));
                                cmd.Parameters.Add(new SqlParameter("@ValueNumber", aggr.ValueNumber));
                                cmd.Parameters.Add(new SqlParameter("@ValueDecimal", aggr.ValueDecimal));
                                cmd.Parameters.Add(new SqlParameter("@ValueBoolean", aggr.ValueBoolean));
                                cmd.Parameters.Add(new SqlParameter("@AnswerId", aggr.AnswerId));
                                cmd.Parameters.Add(new SqlParameter("@ValueCatalogItemId", aggr.ValueCatalogItemId));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggr.TimeStamp));
                                cmd.ExecuteNonQuery();
                            }
                        });
                   
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
