namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Admin;
    using Altea.Classes.Group;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Admin;

    public abstract class GroupService : Service<IGroupChannel>
    {
        public static IEnumerable<AlteaGroup> GetGroups(Guid appId, Language from, Language to, bool onlyActive)
        {
            AlteaGroup[] groups = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetGroups]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

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
                    "@only_active",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    onlyActive);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            Dictionary<Guid, AlteaGroup> allGroups = new Dictionary<Guid, AlteaGroup>();
                            Dictionary<long, AlteaGroupLevel> allGroupLevels  = new Dictionary<long, AlteaGroupLevel>();

                            while (reader.Read())
                            {
                                AlteaGroup group = new AlteaGroup
                                    {
                                        Id = (Guid)reader["id"],
                                        Name = (string)reader["name"],
                                        Levels = new List<AlteaGroupLevel>(),
                                    };

                                allGroups.Add(group.Id, group);
                            }

                            reader.NextResult();

                            Guid lastGroupId = Guid.Empty;
                            List<AlteaGroupLevel> lastGroupLevels = null;
                            while (reader.Read())
                            {
                                long id = (long)reader["id"];
                                Guid group = (Guid)reader["group"];
                                    
                                if (lastGroupId != group)
                                {
                                    lastGroupId = group;
                                    lastGroupLevels = (List<AlteaGroupLevel>)allGroups[group].Levels;
                                }

                                AlteaGroupLevel level = new AlteaGroupLevel
                                    {
                                        Id = id,
                                        Level = (int)reader["level"],
                                        Primary = (bool)reader["primary"],
                                        Active = (bool)reader["active"],
                                        StartDate = reader["start_date"] as DateTime?,
                                        EndDate = reader["end_date"] as DateTime?,
                                        Notes = reader["notes"] as string,
                                        ExamCall = reader["exam_call"] as int?,
                                        PlanningFinished = (bool)reader["planning_finished"],
                                        Timetable = new List<AlteaGroupTimetable>()
                                    };

                                lastGroupLevels.Add(level);
                                allGroupLevels.Add(id, level);
                            }

                            reader.NextResult();

                            AlteaGroupLevel lastGroupLevel = null;
                            AlteaGroupTimetable lastTimetable = null;
                            while (reader.Read())
                            {
                                long groupLevelId = (long)reader["id"];
                                if (lastGroupLevel == null || lastGroupLevel.Id != groupLevelId)
                                {
                                    lastGroupLevel = allGroupLevels[groupLevelId];
                                }

                                int weekday = (int)reader["weekday"];
                                int hour = (int)reader["hour"];
                                int minute = (int)reader["minute"];

                                if (lastTimetable == null || lastTimetable.Weekday != weekday
                                    || lastTimetable.Hour != hour || lastTimetable.Minute != minute)
                                {
                                    lastTimetable = new AlteaGroupTimetable
                                        {
                                            Weekday = (int)reader["weekday"],
                                            Hour = (int)reader["hour"],
                                            Minute = (int)reader["minute"],
                                            Duration = (int)reader["duration"],
                                            Classroom = reader["classroom"] as int?,
                                            Teachers = new List<Guid>()
                                        };

                                    ((List<AlteaGroupTimetable>)(lastGroupLevel.Timetable)).Add(lastTimetable);
                                }

                                Guid? teacher = reader["teacher"] as Guid?;

                                if (teacher.HasValue)
                                {
                                    ((List<Guid>)lastTimetable.Teachers).Add(teacher.Value);
                                }
                            }

                            groups = allGroups.Values.ToArray();
                        });
            }

            return groups;
        }

        public static void GetGroupUsers(Guid id)
        {
            
        }

        public static bool SetGroupLevels(Guid appId, Guid groupId, Language from, Language to, IEnumerable<AdminMemberLevel> levels)
        {
            AdminSetMemberLevelsModel model = new AdminSetMemberLevelsModel
            {
                LanguageFrom = from,
                LanguageTo = to,
                Member = groupId,
                Application = appId,
                Levels = levels
            };

            bool status = Execute<bool>("SetGroupLevels", model);
            return status;
        }

        public static AlteaGroupPlanning GetPlanning(Guid id, int level)
        {
            // TODO
            AlteaGroupPlanning planning = new AlteaGroupPlanning
                {
                    Finished = true
                };

            return planning;
        }
    }
}
