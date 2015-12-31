namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Common.Classes;
    using Altea.Database;
    using Altea.Extensions;

    public abstract class AppService
    {
        public static IDictionary<Language, IEnumerable<Language>> GetLanguages(Guid appId)
        {
            Dictionary<Language, IEnumerable<Language>> languages = new Dictionary<Language, IEnumerable<Language>>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetLanguages]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                Language languageFrom = ((int)reader["from"]).ParseWordLanguageDatabaseId();
                                List<Language> languagesTo;

                                if (!languages.TryGetTypedValue(languageFrom, out languagesTo))
                                {
                                    languagesTo = new List<Language>();
                                    languages.Add(languageFrom, languagesTo);
                                }

                                languagesTo.Add(((int)reader["to"]).ParseWordLanguageDatabaseId());
                            }
                        });
            }

            return languages;
        }

        public static IEnumerable<Level> GetLevels(Language language)
        {
            Dictionary<int, Level> allLevels = new Dictionary<int, Level>();
            List<Level> levels = new List<Level>();

            const string SqlQuery =
                "SELECT * "
                + "FROM [dbo].[altea_GetLevels](@language) "
                + "ORDER BY [level], [parent], [position];";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            Level parentLevel = null;

                            while (reader.Read())
                            {
                                Level level = new Level
                                    {
                                        Id = (int)reader["id"],
                                        Position = (int)reader["position"],
                                        LanguageFrom = language,
                                        Name = (string)reader["name"],
                                        UserDisplayName = (string)reader["user_name"],
                                        AdminDisplayName = (string)reader["admin_name"],
                                        IsCategory = (bool)reader["is_category"],
                                        Selectable = (bool)reader["selectable"],
                                        Image = reader["image"] as string
                                    };

                                int? languageTo = reader["language_to"] as int?;

                                level.LanguageTo = languageTo.HasValue
                                                       ? languageTo.Value.ParseWordLanguageDatabaseId()
                                                       : Language.NoLanguage;

                                allLevels.Add(level.Id, level);

                                int? parent = reader["parent"] as int?;
                                if (parent.HasValue)
                                {
                                    if (parentLevel == null || parentLevel.Id != parent.Value)
                                    {
                                        allLevels.TryGetValue(parent.Value, out parentLevel);
                                    }

                                    if (parentLevel != null)
                                    {
                                        if (parentLevel.Children == null)
                                        {
                                            parentLevel.Children = new List<Level>();
                                        }

                                        ((List<Level>)parentLevel.Children).Add(level);
                                    }
                                }
                                else
                                {
                                    levels.Add(level);
                                }
                            }
                        });
            }

            return levels;
        }

        public static Level GetLevel(int id)
        {
            return GetLevel(Language.NoLanguage, id);
        }

        public static Level GetLevel(Language language, int id)
        {
            Level level = null;

            const string SqlQuery =
                "SELECT * "
                + "FROM [dbo].[altea_GetLevels](@language) "
                + "WHERE [id] = @id "
                + "ORDER BY [level], [parent], [position];";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language == Language.NoLanguage ? (int?)null : language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    id);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            level = new Level
                            {
                                Id = (int)reader["id"],
                                Position = (int)reader["position"],
                                LanguageFrom = (Language)reader["language_from"],
                                Name = (string)reader["name"],
                                UserDisplayName = (string)reader["user_name"],
                                AdminDisplayName = (string)reader["admin_name"],
                                IsCategory = (bool)reader["is_category"],
                                Selectable = (bool)reader["selectable"],
                                Image = reader["image"] as string
                            };

                            int? languageTo = reader["language_to"] as int?;

                            level.LanguageTo = languageTo.HasValue
                                                   ? languageTo.Value.ParseWordLanguageDatabaseId()
                                                   : Language.NoLanguage;
                        }
                    });
            }

            return level;
        }

        public static void GetExamCalls()
        {
            
        }
    }
}
