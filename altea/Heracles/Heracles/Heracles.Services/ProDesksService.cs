namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.Desks;
    using Altea.Classes.ProDesks;
    using Altea.Common.Classes;
    using Altea.Database;
    using Altea.Extensions;

    public abstract class ProDesksService
    {
        public static IEnumerable<ProLevel> GetLevels(Language language)
        {
            Dictionary<int, ProLevel> allLevels = new Dictionary<int, ProLevel>();
            List<ProLevel> levels = new List<ProLevel>();

            const string SqlQuery =
                "SELECT * "
                + "FROM [dbo].[altea_GetProLevels](@language) "
                + "ORDER BY [level], [parent], [position], [sub_position];";
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
                            ProLevel lastLevel = null;
                            ProLevel parentLevel = null;

                            while (reader.Read())
                            {
                                int id = (int)reader["id"];

                                if (lastLevel == null || lastLevel.Id != id)
                                {
                                    lastLevel = new ProLevel
                                        {
                                            Id = id,
                                            Position = (int)reader["position"],
                                            LanguageFrom = language,
                                            Name = (string)reader["name"],
                                            UserDisplayName = (string)reader["user_name"],
                                            AdminDisplayName = (string)reader["admin_name"],
                                            IsCategory = (bool)reader["is_category"],
                                            Selectable = (bool)reader["selectable"],
                                            ForceAcademicLevel = (bool)reader["force_academic_level"],
                                            Image = reader["image"] as string
                                        };

                                    int? languageTo = reader["language_to"] as int?;
                                    lastLevel.LanguageTo = languageTo.HasValue
                                                               ? languageTo.Value.ParseWordLanguageDatabaseId()
                                                               : Language.NoLanguage;

                                    allLevels.Add(id, lastLevel);

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
                                                parentLevel.Children = new List<ProLevel>();
                                            }

                                            ((List<ProLevel>)parentLevel.Children).Add(lastLevel);
                                        }
                                    }
                                    else
                                    {
                                        levels.Add(lastLevel);
                                    }
                                }

                                if (!lastLevel.IsCategory && lastLevel.Selectable)
                                {
                                    int subId = (int)reader["sub_id"];

                                    if (subId != 0 || !lastLevel.ForceAcademicLevel)
                                    {
                                        ProLevel subLevel = new ProLevel
                                            {
                                                Id = subId,
                                                Position = (int)reader["sub_position"],
                                                Name = (string)reader["sub_name"],
                                                UserDisplayName = (string)reader["sub_user_name"],
                                                AdminDisplayName =
                                                    (string)reader["sub_admin_name"],
                                                IsCategory = false,
                                                Selectable = true
                                            };

                                        if (lastLevel.SubLevels == null)
                                        {
                                            lastLevel.SubLevels = new List<ProLevel>();
                                        }

                                        ((List<ProLevel>)lastLevel.SubLevels).Add(subLevel);
                                    }
                                }
                            }
                        });
            }

            return levels;
        }

        public static ProLevel GetLevel(Language language, int id, int? subId)
        {
            ProLevel level = null;

            const string SqlQuery =
                "SELECT * "
                + "FROM [dbo].[altea_GetProLevels](@language) "
                + "WHERE [id] = @id AND [is_category] = 0 AND [selectable] = 1 "
                + "AND ((@sub_id IS NULL AND [sub_id] = 0 AND [force_academic_level] = 0) OR ([sub_id] = @sub_id)) "
                + "ORDER BY [level], [parent], [position], [sub_position];";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sub_id",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                level = new ProLevel
                                    {
                                        Id = (int)reader["id"],
                                        SubId = reader["sub_id"] as int?,
                                        Position = (int)reader["position"],
                                        LanguageFrom = language,
                                        Name = (string)reader["name"],
                                        UserDisplayName = (string)reader["user_name"],
                                        AdminDisplayName = (string)reader["admin_name"],
                                        SubName = reader["sub_name"] as string,
                                        UserDisplaySubName = reader["sub_user_name"] as string,
                                        AdminDisplaySubName = reader["sub_admin_name"] as string,
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

        public static ProDesksList GetList(Language language, int level, int? subLevel, ProDesksQuestionType type)
        {
            ProDesksList list = null;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(
                CommandType.StoredProcedure,
                "[dbo].[PRODESKS_List]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sub_level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subLevel ?? 0);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    type);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            DesksIndexList index = DesksService.GetIndexList(reader);
                            reader.NextResult();
                            DesksExamList exam = DesksService.GetExamsList(reader);

                            list = new ProDesksList
                                {
                                    Index = index,
                                    Exams = exam
                                };
                        });
            }
            
            return list;
        }

        public static IEnumerable<IProDesksAssignment> GetAssignments(Guid userId, int level, int? subLevel)
        {
            List<IProDesksAssignment> assignments = new List<IProDesksAssignment>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[PRODESKS_GetAssignments]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sub_level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subLevel ?? 0);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            // Index
                            while (reader.Read())
                            {
                                int assigned = (int)reader["assigned"];
                                int finished = (int)reader["finished"];
                                int certified = (int)reader["certified"];

                                if (assigned + finished + certified == 0)
                                {
                                    continue;
                                }

                                ProDesksIndexAssignment assignment = new ProDesksIndexAssignment
                                    {
                                        Area = (int)reader["area"],
                                        Subject = (int)reader["subject"],
                                        Type = (DesksIndexExerciseType)reader["type"],
                                        Assigned = assigned,
                                        RemoteAssignment = (int)reader["remote_Assignment"],
                                        Blocked = (int)reader["blocked"],
                                        Finished = finished,
                                        Certified = certified
                                    };

                                assignments.Add(assignment);
                            }

                            //reader.NextResult();

                            // Exams
                        });
            }

            return assignments;
        }
    }
}
