namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseLab;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.WiseLab;

    using Heracles.Models.WiseLab;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;

    public abstract class WiseLabService : Service<IWiseLabChannel>
    {
        private static readonly int WiseLabConnectionRetries;

        private static readonly int WiseLabConnectionRetryWaitSeconds;

        static WiseLabService()
        {
            WiseLabService.WiseLabConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["WiseLabConnectionRetries"]);

            WiseLabService.WiseLabConnectionRetryWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["WiseLabConnectionRetryWaitSeconds"]);
        }

        public static WiseLabArticle GetArticle(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference)
        {
            WiseLabArticle article = new WiseLabArticle { Origin = origin, Reference = reference };

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[WISELAB_GetArticle]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@origin",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    origin);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@reference",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    reference);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                article.Status = WiseLabStatus.None;
                                return;
                            }

                            article.Status = (WiseLabStatus)reader["status"];
                            article.ForcePod = (bool)reader["force_pod"];
                            article.Lead = reader["lead"] as string;

                            reader.NextResult();

                            List<WiseLabHuntData> huntData = new List<WiseLabHuntData>();
                            while (reader.Read())
                            {
                                WiseLabHuntData data = new WiseLabHuntData
                                    {
                                        Type = (WiseLabHuntType)reader["type"],
                                        Data = (string)reader["data"],
                                        Sentence = reader["sentence"] as string,
                                        HuntDate = (DateTime)reader["hunt_date"]
                                    };

                                huntData.Add(data);

                                if (data.Type == WiseLabHuntType.Scout)
                                {
                                    data.Searched = (bool)reader["searched"];
                                }
                            }

                            article.HuntData = huntData;
                        });
            }

            return article;
        }

        public static WiseLabError SearchWord(
            Guid userId,
            Language from,
            Language to,
            WiseLabOrigin origin,
            int reference,
            string data,
            int inboxOverflow,
            int offsetDate)
        {

            WiseLabHuntDataModel model = new WiseLabHuntDataModel
            {
                UserId = userId,
                LanguageFrom = from,
                LanguageTo = to,
                Origin = origin,
                Reference = reference,
                Data = data,
                InboxOverflow = inboxOverflow,
                OffsetDate = offsetDate
            };

            return AddHuntData(model, true);
        }

        public static WiseLabError AddHuntData(Guid userId,
            Language from,
            Language to,
            WiseLabOrigin origin,
            int reference,
            string data,
            string sentence,
            int inboxOverflow,
            int offsetDate)
        {
            WiseLabHuntDataModel model = new WiseLabHuntDataModel
            {
                UserId = userId,
                LanguageFrom = from,
                LanguageTo = to,
                Origin = origin,
                Reference = reference,
                Data = data,
                Sentence = sentence,
                InboxOverflow = inboxOverflow,
                OffsetDate = offsetDate
            };
            return AddHuntData(model, false);
        }

        static WiseLabError AddHuntData(WiseLabHuntDataModel model, bool searched)
        {
           
            WiseLabError error;

            string normalizedData = model.Data.Trim();
            string normalizedSentence = model.Sentence == null ? null : model.Sentence.Trim();
            if (normalizedSentence != null && normalizedSentence.Length == 0)
            {
                normalizedSentence = null;
            }

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISELAB_AddHuntData]"))
            {
                AddGlobalParameters(command, model);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sentence",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedSentence);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@searched",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    searched);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@inbox_overflow",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.InboxOverflow);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@error",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                error = (WiseLabError)command.Parameters["@error"].Value;
            }

            return error;
        }

        static void AddGlobalParameters(SqlCommand command, WiseLabArticleDataModel model)
        {
            SqlDatabaseManager.AddParameter(
                command,
                "@uid",
                ParameterDirection.Input,
                SqlDbType.UniqueIdentifier,
                model.UserId);

            SqlDatabaseManager.AddParameter(
                command,
                "@language_from",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.LanguageFrom.GetDatabaseId());

            SqlDatabaseManager.AddParameter(
                command,
                "@language_to",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.LanguageTo.GetDatabaseId());

            SqlDatabaseManager.AddParameter(
                command,
                "@origin",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Origin);

            SqlDatabaseManager.AddParameter(
                command,
                "@reference",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Reference);
        }

        public static WiseLabError RemoveHuntData(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data)
        {
            WiseLabHuntDataModel model = new WiseLabHuntDataModel
                {
                    UserId = userId,
                    LanguageFrom = from,
                    LanguageTo = to,
                    Origin = origin,
                    Reference = reference,
                    Data = data
                };

            WiseLabError error = Execute<WiseLabError>("RemoveHuntData", model);
            return error;
        }

        public static WiseLabError SaveLead(
            Guid userId,
            Language from,
            Language to,
            WiseLabOrigin origin,
            int reference,
            string lead,
            bool autoSave)
        {
            WiseLabWisdomHunterModel model = new WiseLabWisdomHunterModel
                {
                    UserId = userId,
                    LanguageFrom = from,
                    LanguageTo = to,
                    Origin = origin,
                    Reference = reference,
                    Lead = lead,
                    AutoSave = autoSave
                };

            WiseLabError error = Execute<WiseLabError>("SaveLead", model);
            return error;
        }

        public static WiseLabStatus FinishStatus(
            Guid userId,
            Language from,
            Language to,
            WiseLabOrigin origin,
            int reference,
            int offsetDate)
        {
            WiseLabArticleDataModel model = new WiseLabArticleDataModel
                {
                    UserId = userId,
                    LanguageFrom = from,
                    LanguageTo = to,
                    Origin = origin,
                    Reference = reference,
                    OffsetDate = offsetDate
                };

            WiseLabStatus newStatus = Execute<WiseLabStatus>("FinishStatus", model);
            return newStatus;
        }



        public static void LogException(
            Guid userId,
            Language from,
            Language to,
            WiseLabOrigin origin,
            int reference,
            string stack,
            string message,
            string word,
            string context,
            string parent)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["WiseLabStorage"].ConnectionString;
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            client.DefaultRequestOptions.RetryPolicy =
                    new LinearRetry(
                        TimeSpan.FromSeconds(WiseLabService.WiseLabConnectionRetryWaitSeconds),
                        WiseLabService.WiseLabConnectionRetries);
            CloudTable table = client.GetTableReference("exceptions");
            table.CreateIfNotExists();

            WiseLabExceptionEntity row = new WiseLabExceptionEntity(DateTime.UtcNow)
            {
                UserId = userId,
                LanguageFrom = from.GetPrefix(LanguagePrefixType.LongName),
                LanguageTo = to.GetPrefix(LanguagePrefixType.LongName),
                Origin = origin.ToString(),
                Reference = reference,
                ErrorStack = stack,
                ErrorMessage = message,
                Word = word,
                Context = context,
                Parent = parent,
                Fixed = false
            };

            TableOperation insert = TableOperation.Insert(row);
            table.Execute(insert);
        }
    }
}
