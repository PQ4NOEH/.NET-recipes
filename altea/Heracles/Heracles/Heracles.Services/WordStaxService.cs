namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Members;
    using Altea.Classes.Stax;
    using Altea.Classes.Stax.WordStax;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;

    /// <summary>
    /// The wordstax service.
    /// </summary>
    public abstract partial class WordStaxService : Service<IStaxChannel>
    {
        /// <summary>
        /// The min stack.
        /// </summary>
        public const int MinStack = 1;

        /// <summary>
        /// The max stack.
        /// </summary>
        public const int MaxStack = 7;

        #region Inbox

        public static IEnumerable<WordStaxInboxData> GetInbox(Guid userId, Language language)
        {
            List<WordStaxInboxData> inbox = new List<WordStaxInboxData>(40);

            const string SqlQuery =
                "SELECT [id], [origin], [insert_date], [data], [num_errors], [reinserted] " +
                "FROM [dbo].[VOCABULARY_Inbox] " +
                "WHERE [user] = @user AND [language] = @language " +
                "AND [accepted] = CAST(0 AS [bit]) AND [rejected] = CAST(0 AS [bit]);";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier, 
                    userId);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command, 
                    SqlConnectionString.DataWarehouse, 
                    reader =>
                        {
                            while (reader.Read())
                            {
                                WordStaxInboxData data = new WordStaxInboxData
                                {
                                    Id = (long)reader["id"], 
                                    Origin = (int)reader["origin"], 
                                    InsertDate = (DateTime)reader["insert_date"], 
                                    Data = (string)reader["data"], 
                                    NumErrors = (int)reader["num_errors"], 
                                    Reinserted = (bool)reader["reinserted"]
                                };

                                inbox.Add(data);
                            }
                        });
            }

            return inbox;
        }

        /// <summary>
        /// The check inbox data.
        /// </summary>
        public static void CheckInboxData()
        {
            
        }

        #endregion



        #region Stax

        public static IDictionary<string, IEnumerable<string>> CheckWordsInStax(Guid user, Language from, Language to, IEnumerable<string> words)
        {
            string[] normalizedWords = words.Select(x => x.ToLowerInvariant()).Distinct().ToArray();
            Dictionary<string, IEnumerable<string>> wordsInStax = new Dictionary<string, IEnumerable<string>>();

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[VOCABULARY_CheckWordsInStax]"))
            using (DataTable wordsTable = new DataTable())
            {
                wordsTable.Columns.Add("word", typeof(string));

                foreach (string word in normalizedWords)
                {
                    DataRow row = wordsTable.NewRow();
                    row["word"] = word;
                    wordsTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier, 
                    user);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_from", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    (int)from);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_to", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    (int)to);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@words", 
                    ParameterDirection.Input, 
                    "[dbo].[wordlist]", 
                    wordsTable);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse, 
                    reader =>
                        {
                            string lastWord = null;
                            HashSet<string> lastTranslations = null;

                            while (reader.Read())
                            {
                                string word = (string)reader["word"];
                                string translation = (string)reader["translation"];

                                if (word != lastWord)
                                {
                                    lastWord = word;
                                    if (wordsInStax.ContainsKey(lastWord))
                                    {
                                        lastTranslations = (HashSet<string>)wordsInStax[lastWord];
                                    }
                                    else
                                    {
                                        lastTranslations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                        wordsInStax.Add(lastWord, lastTranslations);
                                    }
                                }

                                lastTranslations.Add(translation);
                            }
                        });
            }

            return wordsInStax;
        }

        public static IEnumerable<Stack> GetStax(Guid userId, Language languageFrom, Language languageTo)
        {
            List<Stack> stax = new List<Stack>(MaxStack);

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[VOCABULARY_GetStax]"))
            {
                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier, 
                    userId);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_from", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_to", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageTo.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@max_stack",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    MaxStack);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse, 
                    reader =>
                        {
                            while (reader.Read())
                            {
                                Stack stack = new Stack
                                    {
                                        Number = (int)reader["stack"], 
                                        Success = (int)reader["success"], 
                                        Errors = (int)reader["errors"],
                                        Mean = (TimeSpan)reader["mean"],
                                        Data = (int)reader["data"]
                                    };

                                stax.Add(stack);
                            }
                        });
            }

            return stax;
        }

        private static int GetStackCount(
            User user, 
            Language from, 
            Language to, 
            int stack,
            IEnumerable<long> excludedData)
        {
            const string SqlQuery =
                "SELECT COUNT(*) FROM [dbo].[VOCABULARY_StaxData] AS [A] " +
                "INNER JOIN [dbo].[VOCABULARY_Stax] AS [B] ON [B].[id] = [A].[stack] " +
                "WHERE [A].[user] = @uid AND [A].[language_from] = @from AND [A].[language_to] = @to " +
                "AND [B].[stack] = @stack AND [A].[id] NOT IN (SELECT [n] FROM @excluded);";

            using (DataTable excludedDataTable = new DataTable())
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                excludedDataTable.Columns.Add("n", typeof(long));

                foreach (long id in excludedData)
                {
                    DataRow row = excludedDataTable.NewRow();
                    row["n"] = id;
                    excludedDataTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, user.Id);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, from.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, to.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack", ParameterDirection.Input, SqlDbType.Int, stack);
                SqlDatabaseManager.AddParameter(command, "@excluded", ParameterDirection.Input, "[dbo].[bigintlist]", excludedDataTable);

                return SqlDatabaseManager.ExecuteScalar<int>(command, SqlConnectionString.DataWarehouse);
            }
        }

        public static bool CheckStackUnderflow(
            User user, 
            Language from, 
            Language to, 
            int stack, 
            IEnumerable<long> excludedData)
        {
            int count = GetStackCount(user, from, to, stack, excludedData);
            return count < Convert.ToInt32(user["vocabulary_stax_underflow"]);
        }

        public static bool CheckStackOverflow(
            User user, 
            Language from, 
            Language to, 
            int stack, 
            IEnumerable<long> excludedData)
        {
            int count = GetStackCount(user, from, to, stack, excludedData);
            return count >= Convert.ToInt32(user["vocabulary_stax_overflow"]);
        }

        public static IStackFormula NewExercise(
            Guid userId,
            Guid appId,
            Language from, 
            Language to, 
            int stack,
            int numberData,
            int extraData,
            int maxData,
            IEnumerable<long> excludedData)
        {
            IStackFormula exercise = null;
            Func<SqlDataReader, int, int, IStackFormula> formula;

            switch (stack)
            {
                case 1:
                case 2:
                    formula = WordStaxService.WordStack;
                    break;

                case 3:
                case 4:
                    formula = WordStaxService.AudioWordStack;
                    break;

                case 5:
                case 6:
                    formula = WordStaxService.ExtendedWordStack;
                    break;

                case 7:
                    formula = WordStaxService.SentenceWordStack;
                    break;

                default:
                    throw new NotImplementedException();
            }

            using (DataTable excludedDataTable = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[VOCABULARY_NewExercise]"))
            {
                excludedDataTable.Columns.Add("n", typeof(long));

                foreach (long data in excludedData)
                {
                    DataRow row = excludedDataTable.NewRow();
                    row["n"] = data;
                    excludedDataTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stack",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    stack);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@number_data",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    numberData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@extra_data",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    extraData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@max_data",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    maxData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@excluded",
                    ParameterDirection.Input,
                    "[dbo].[bigintlist]",
                    excludedDataTable);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            exercise = formula(reader, numberData, extraData);
                        });
            }

            return exercise;
        }

        #endregion



        #region Finished

        public static string[] GetFinishedWords(Guid userId, Language languageFrom, Language languageTo)
        {
            List<string> words = new List<string>();

            const string SqlQuery =
                "SELECT DISTINCT LOWER(LTRIM(RTRIM([data]))) AS [data] " +
                "FROM [dbo].[VOCABULARY_StaxData] " +
                "WHERE [user] = @user " +
                "AND [language_from] = @language_from " +
                "AND [language_to] = @language_to " +
                "AND [finished] = CAST(1 AS [bit]) " +
                "AND [failed] = CAST(0 AS [bit]);";

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier, 
                    userId);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_from", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_to", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageTo.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse, 
                    reader =>
                        {
                            while (reader.Read())
                            {
                                words.Add((string)reader["data"]);
                            }
                        });
            }

            return words.ToArray();
        }

        public static int CountFinishedData(Guid userId, Language languageFrom, Language languageTo)
        {
            int finished;

            const string SqlQuery =
                "SELECT COUNT(*) " +
                "FROM [dbo].[VOCABULARY_StaxData] " +
                "WHERE [user] = @user " +
                "AND [language_from] = @language_from " +
                "AND [language_to] = @language_to " +
                "AND [finished] = CAST(1 AS [bit]) " +
                "AND [failed] = CAST(0 AS [bit]);";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier, 
                    userId);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_from", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@language_to", 
                    ParameterDirection.Input, 
                    SqlDbType.Int, 
                    languageTo.GetDatabaseId());

                finished = SqlDatabaseManager.ExecuteScalar<int>(command, SqlConnectionString.DataWarehouse);
            }

            return finished;
        }

        /// <summary>
        /// The get finished page.
        /// </summary>
        public static void GetFinishedPage()
        {
            
        }

        #endregion
    }
}
