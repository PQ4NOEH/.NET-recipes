using System;
using AlteaLabs.Common.Contracts;
using AlteaLabs.WiseLab.Contracts;
using System.Data.SqlClient;
using System.Data;

namespace AlteaLabs.WiseLab.Services
{
    public class WiseLabService : IWiseLabService
    {
        public WiseLabError AddHuntData(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data, string sentence, int inboxOverflow, int offsetDate)
        {
            throw new NotImplementedException();
        }

        public WiseLabStatus FinishStatus(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, int offsetDate)
        {
            throw new NotImplementedException();
        }

        public WiseLabArticle GetArticle(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference)
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

        public WiseLabError RemoveHuntData(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data)
        {
            throw new NotImplementedException();
        }

        public WiseLabError SaveLead(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string lead, bool autoSave)
        {
            throw new NotImplementedException();
        }

        public WiseLabError SearchWord(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data, int inboxOverflow, int offsetDate)
        {
            throw new NotImplementedException();
        }
    }
}
