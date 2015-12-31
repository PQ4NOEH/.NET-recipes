namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Stax;
    using Altea.Classes.Stax.WordStax;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Stax;

    /// <summary>
    /// The stax service.
    /// </summary>
    public abstract class StaxService : Service<IStaxChannel>
    {
        #region Inbox

        /// <summary>
        /// The insert inbox data.
        /// </summary>
        public static int InsertInboxData(
            Guid user,
            Language from,
            StackType stackType,
            WordStaxOrigin origin,
            int? reference,
            string data,
            bool searched,
            string sentence,
            int inboxOverflow,
            int offsetDate)
        {
            StackNewInboxDataModel model = new StackNewInboxDataModel
            {
                UserId = user,
                From = from,
                To = from,
                Type = stackType,
                Data = data,
                Origin = (int)origin,
                Searched = searched,
                Sentence = sentence,
                InboxOverflow = inboxOverflow,
                OffsetDate = offsetDate
            };

            int id = StaxService.Execute<int>("InsertInboxData", model);
            return id;
        }

        /// <summary>
        /// The accept inbox data.
        /// </summary>
        public static void AcceptInboxData(
            Guid user,
            Language from,
            Language to,
            StackType stackType,
            long id,
            string data,
            int maxData,
            IEnumerable<StaxContentData> dataOptions,
            int offsetDate)
        {
            StackAcceptInboxDataModel model = new StackAcceptInboxDataModel
            {
                UserId = user,
                From = from,
                To = to,
                Type = StackType.Vocabulary,
                Id = id,
                Data = data,
                MaxData = maxData,
                DataOptions = dataOptions,
                OffsetDate = offsetDate
            };

            StaxService.Execute("AcceptInboxData", model);
        }

        /// <summary>
        /// The delete inbox data.
        /// </summary>
        public static void DeleteInboxData(
            Guid user,
            Language from,
            StackType stackType,
            long id,
            string data)
        {
            StackDeleteInboxDataModel model = new StackDeleteInboxDataModel
            {
                UserId = user,
                From = from,
                To = from,
                Type = stackType,
                Id = id,
                Data = data
            };

            StaxService.Execute("DeleteInboxData", model);
        }

        #endregion



        #region Graphs

        public static IEnumerable<StaxWeekGraph> GetWeekGraphs(
            Guid userId,
            Language languageFrom,
            Language languageTo,
            StackType stackType,
            IEnumerable<string> graphTypes,
            DateTime todayDate,
            int weekStartDay,
            int offsetDate)
        {
            List<StaxWeekGraph> graphs = new List<StaxWeekGraph>(graphTypes.Count());

            using (DataTable graphTypesTable = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STAX_GetWeekGraphs]"))
            {
                graphTypesTable.Columns.Add("n", typeof(string));

                foreach (string type in graphTypes)
                {
                    DataRow row = graphTypesTable.NewRow();
                    row["n"] = type;
                    graphTypesTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, userId);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, languageFrom.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, languageTo.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)stackType);
                SqlDatabaseManager.AddParameter(command, "@graph_types", ParameterDirection.Input, "[dbo].[stringlist]", graphTypesTable);
                SqlDatabaseManager.AddParameter(command, "@today_date", ParameterDirection.Input, SqlDbType.DateTime, todayDate);
                SqlDatabaseManager.AddParameter(command, "@week_start_day", ParameterDirection.Input, SqlDbType.Int, weekStartDay);
                SqlDatabaseManager.AddParameter(command, "@offset_date", ParameterDirection.Input, SqlDbType.Int, offsetDate);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            do
                            {
                                StaxWeekGraph graph = ReadWeekGraph(reader);
                                graphs.Add(graph);
                            }
                            while (reader.NextResult());
                        });
            }

            return graphs;
        }

        private static StaxWeekGraph ReadWeekGraph(SqlDataReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }

            StaxWeekGraph graph = new StaxWeekGraph { Name = (string)reader["graph_type"] };
            List<StackDayGraph> thisWeek = (List<StackDayGraph>)graph.ThisWeek;
            List<StackDayGraph> lastWeek = (List<StackDayGraph>)graph.LastWeek;
            List<StackDayGraph> averageWeek = (List<StackDayGraph>)graph.AverageWeek;

            do
            {
                int weekDay = (int)reader["weekday"];

                thisWeek.Add(new StackDayGraph { Weekday = weekDay, Count = reader["this_week"] as int? ?? 0 });
                lastWeek.Add(new StackDayGraph { Weekday = weekDay, Count = reader["last_week"] as int? ?? 0 });
                averageWeek.Add(new StackDayGraph { Weekday = weekDay, Count = reader["average_week"] as int? ?? 0 });
            }
            while (reader.Read());

            return graph;
        }

        #endregion



        #region Stax

        public static void CheckStaxExist(
            Guid userId,
            Language languageFrom,
            Language languageTo,
            StackType stackType,
            int maxStack)
        {
            StaxCheckModel model = new StaxCheckModel
                {
                    UserId = userId,
                    LanguageFrom = languageFrom,
                    LanguageTo = languageTo,
                    Type = stackType,
                    MaxStack = maxStack
                };

            StaxService.Execute("CheckStaxExist", model);
        }

        public static IEnumerable<string> SaveExercise(
            Guid userId,
            Guid appId,
            bool remote,
            Language from,
            Language to,
            StackType stackType,
            int stack,
            IEnumerable<StackExerciseAnswer> exercises,
            bool status,
            TimeSpan time,
            int retryCooldown,
            int inboxErrors,
            int offsetDate)
        {
            StackSaveExerciseDataModel model = new StackSaveExerciseDataModel
            {
                UserId = userId,
                AppId = appId,
                Remote = remote,
                From = from,
                To = to,
                Type = stackType,
                StackNum = stack,
                Exercises = exercises,
                Status = status,
                Time = time,
                RetryCooldown = retryCooldown,
                InboxErrors = inboxErrors,
                OffsetDate = offsetDate
            };

            IEnumerable<string> inbox = StaxService.Execute<IEnumerable<string>>("SaveExercise", model);
            return inbox;
        }

        #endregion
    }
}
