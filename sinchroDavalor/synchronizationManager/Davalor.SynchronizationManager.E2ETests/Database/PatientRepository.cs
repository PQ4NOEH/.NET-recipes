using Davalor.PortalPaciente.Messages.Patient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class PatientRepository : BaseRepository<PatientAggregate>
    {
        public PatientRepository(string DbConnectionString) : base(DbConnectionString) { }
        public PatientAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[PersonId],[UserId],[SapCode],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Patient]" + 
                "where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new PatientAggregate
                        {
                            Id =        r.GetGuid(0),
                            PersonId =  r.GetGuid(1),
                            UserId =    r.GetGuid(2),
                            SapCode =   r.GetString(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT [Id],[Name],[Surname1],[Surname2],[Gender],[BirthDate],[Email],[Phone1],[Phone2],[DocumentTypeId],"+
                "[DocumentIdentifier],[NationalityId],[TitleId],[LanguageId],[CurrencyId],[Deleted],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[Person]" +
                "where Id = '" + resultGraph.PersonId.ToString() + "'";
            resultGraph.Person = this.Query(query, r =>
                {
                    return new PatientAggregate
                        {
                            Person = new Person
                            {
                                Id                 = r.GetGuid(0),
                                Name               = r.GetString(1),
                                Surname1           = r.GetString(2),
                                Surname2           = r.GetString(3),
                                Gender             = r.GetInt32(4),
                                BirthDate          = r.GetDateTime(5),
                                Email              = r.GetString(6),
                                Phone1             = r.GetString(7),
                                Phone2             = r.GetString(8),
                                DocumentTypeId     = r.GetGuid(9),
                                DocumentIdentifier = r.GetString(10),
                                NationalityId       = r.GetGuid(11),
                                TitleId            = r.GetGuid(12),
                                LanguageId         = r.GetGuid(13),
                                CurrencyId         = r.GetGuid(14),
                                Deleted            = r.GetInt32(15),
                                TimeStamp          = r.GetDateTimeOffset(16)
                            }
                        };
                }).Person;
            query = "SELECT [Id],[UserName],[Hash],[TokenHash],[RetryCount],[Active],[Locked],[RecordDeleted],[RecordDeletedDate],[Email]," +
                "[NewEmail],[NewEmailCode],[NewEmailRequest],[ForgotPasswordCode],[ForgotPasswordRequest],[LastChangePassword],[LastLogon], "+
                "[RegistrationCode],[RegistrationDate],[LanguageId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[User]" +
                " where Id = '" + resultGraph.UserId.ToString() + "'";
            resultGraph.User = this.Query(query, r =>
            {
                return new PatientAggregate
                {
                    User = new User
                    {
                        Id = r.GetGuid(0),
                        UserName = r.GetString(1),
                        Hash = r.GetString(2),
                        TokenHash = r.GetString(3),
                        RetryCount = r.GetInt32(4),
                        Active = r.GetInt32(5),
                        Locked = r.GetInt32(6),
                        RecordDeleted = r.GetInt32(7),
                        RecordDeletedDate = r.GetDateTimeOffset(8),
                        Email = r.GetString(9),
                        NewEmail = r.GetString(10),
                        NewEmailCode = r.GetString(11),
                        NewEmailRequest = r.GetDateTimeOffset(12),
                        ForgotPasswordCode = r.GetString(13),
                        ForgotPasswordRequest = r.GetDateTimeOffset(14),
                        LastChangePassword = r.GetDateTimeOffset(15),
                        LastLogon = r.GetDateTimeOffset(16),
                        RegistrationCode = r.GetString(17),
                        RegistrationDate = r.GetDateTimeOffset(18),
                        LanguageId = r.GetGuid(19),
                        TimeStamp = r.GetDateTimeOffset(20)
                    }
                    
                };
            }).User;
            query = "SELECT [Id],[PersonId],[LocationId],[TimeStamp] FROM [VisionLocalIntegrationTests].[dbo].[PersonLocation]" +
                "where PersonId = '" + resultGraph.Person.Id.ToString() + "'";

            resultGraph.Person.PersonLocation = this.Query(query, r =>
            {
                return new PatientAggregate
                {
                    Person = new Person
                    {
                        PersonLocation = new Collection<PersonLocation>
                        {
                            new PersonLocation
                            {
                                Id = r.GetGuid(0),
                                PersonId = r.GetGuid(1),
                                LocationId =r.GetGuid(2),
                                TimeStamp = r.GetDateTimeOffset(3)
                            }
                        }
                    }
                };
            }).Person.PersonLocation;
            return resultGraph;
        }

        public void Insert(PatientAggregate aggregate)
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
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Person] "+
                            "([Id],[Name],[Surname1],[Surname2],[Gender],[BirthDate],[Email],[Phone1],[Phone2],[DocumentTypeId],[DocumentIdentifier],[NationalityId],[TitleId],[LanguageId],[CurrencyId],[Deleted],[TimeStamp])" +
                            " VALUES (@Id,@Name,@Surname1,@Surname2,@Gender,@BirthDate,@Email,@Phone1,@Phone2,@DocumentTypeId,@DocumentIdentifier,@NationalityId,@TitleId,@LanguageId,@CurrencyId,@Deleted,@TimeStamp )";

                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Person.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", aggregate.Person.Name));
                        cmd.Parameters.Add(new SqlParameter("@Surname1", aggregate.Person.Surname1));
                        cmd.Parameters.Add(new SqlParameter("@Surname2", aggregate.Person.Surname2));
                        cmd.Parameters.Add(new SqlParameter("@Gender", aggregate.Person.Gender));
                        cmd.Parameters.Add(new SqlParameter("@BirthDate", aggregate.Person.BirthDate));
                        cmd.Parameters.Add(new SqlParameter("@Email", aggregate.Person.Email));
                        cmd.Parameters.Add(new SqlParameter("@Phone1", aggregate.Person.Phone1));
                        cmd.Parameters.Add(new SqlParameter("@Phone2", aggregate.Person.Phone2));
                        cmd.Parameters.Add(new SqlParameter("@DocumentTypeId", aggregate.Person.DocumentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@DocumentIdentifier", aggregate.Person.DocumentIdentifier));
                        cmd.Parameters.Add(new SqlParameter("@NationalityId", aggregate.Person.NationalityId));
                        cmd.Parameters.Add(new SqlParameter("@TitleId", aggregate.Person.TitleId));
                        cmd.Parameters.Add(new SqlParameter("@LanguageId", aggregate.Person.LanguageId));
                        cmd.Parameters.Add(new SqlParameter("@CurrencyId", aggregate.Person.CurrencyId));
                        cmd.Parameters.Add(new SqlParameter("@Deleted", aggregate.Person.Deleted));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.Person.TimeStamp));

                        cmd.ExecuteNonQuery();
                    }
                    aggregate.Person.PersonLocation.ToList().ForEach(pl =>
                        {
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[PersonLocation] ([Id],[PersonId],[LocationId],[TimeStamp])" +
                                    " VALUES (@Id,@PersonId,@LocationId,@TimeStamp)";

                                cmd.Parameters.Add(new SqlParameter("@Id", pl.Id));
                                cmd.Parameters.Add(new SqlParameter("@PersonId", pl.PersonId));
                                cmd.Parameters.Add(new SqlParameter("@LocationId", pl.LocationId));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", pl.TimeStamp));

                                cmd.ExecuteNonQuery();
                            }
                        });
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[User]"+
                            "([Id],[UserName],[Hash],[TokenHash],[RetryCount],[Active],[Locked],[RecordDeleted],[RecordDeletedDate],[Email],[NewEmail],[NewEmailCode],[NewEmailRequest],[ForgotPasswordCode],[ForgotPasswordRequest],[LastChangePassword],[LastLogon],[RegistrationCode],[RegistrationDate],[LanguageId],[TimeStamp] )" +
                            " VALUES (@Id,@UserName,@Hash,@TokenHash,@RetryCount,@Active,@Locked,@RecordDeleted,@RecordDeletedDate,@Email,@NewEmail,@NewEmailCode,@NewEmailRequest,@ForgotPasswordCode,@ForgotPasswordRequest,@LastChangePassword,@LastLogon,@RegistrationCode,@RegistrationDate,@LanguageId,@TimeStamp)";

                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.User.Id));
                        cmd.Parameters.Add(new SqlParameter("@UserName", aggregate.User.UserName));
                        cmd.Parameters.Add(new SqlParameter("@Hash", aggregate.User.Hash));
                        cmd.Parameters.Add(new SqlParameter("@TokenHash", aggregate.User.TokenHash));
                        cmd.Parameters.Add(new SqlParameter("@RetryCount", aggregate.User.RetryCount));
                        cmd.Parameters.Add(new SqlParameter("@Active", aggregate.User.Active));
                        cmd.Parameters.Add(new SqlParameter("@Locked", aggregate.User.Locked));
                        cmd.Parameters.Add(new SqlParameter("@RecordDeleted", aggregate.User.RecordDeleted));
                        cmd.Parameters.Add(new SqlParameter("@RecordDeletedDate", aggregate.User.RecordDeletedDate));
                        cmd.Parameters.Add(new SqlParameter("@Email", aggregate.User.Email));
                        cmd.Parameters.Add(new SqlParameter("@NewEmail", aggregate.User.NewEmail));
                        cmd.Parameters.Add(new SqlParameter("@NewEmailCode", aggregate.User.NewEmailCode));
                        cmd.Parameters.Add(new SqlParameter("@NewEmailRequest", aggregate.User.NewEmailRequest));
                        cmd.Parameters.Add(new SqlParameter("@ForgotPasswordCode", aggregate.User.ForgotPasswordCode));
                        cmd.Parameters.Add(new SqlParameter("@ForgotPasswordRequest", aggregate.User.ForgotPasswordRequest));
                        cmd.Parameters.Add(new SqlParameter("@LastChangePassword", aggregate.User.LastChangePassword));
                        cmd.Parameters.Add(new SqlParameter("@LastLogon", aggregate.User.LastLogon));
                        cmd.Parameters.Add(new SqlParameter("@RegistrationCode", aggregate.User.RegistrationCode));
                        cmd.Parameters.Add(new SqlParameter("@RegistrationDate", aggregate.User.RegistrationDate));
                        cmd.Parameters.Add(new SqlParameter("@LanguageId", aggregate.User.LanguageId));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.User.TimeStamp));

                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Patient] ([Id],[PersonId],[UserId],[SapCode],[TimeStamp])" +
                            " VALUES (@Id,@PersonId,@UserId, @SapCode,@TimeStamp)";

                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@PersonId", aggregate.PersonId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", aggregate.UserId));
                        cmd.Parameters.Add(new SqlParameter("@SapCode", aggregate.SapCode));
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
