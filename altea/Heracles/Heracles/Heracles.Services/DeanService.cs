namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Dean;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;

    public abstract class DeanService : Service<IDeanChannel>
    {
        public static IEnumerable<DeanUser> GetUsers(Guid userId, Guid appId, Language from, Language to)
        {
            Dictionary<Guid, DeanUser> users = new Dictionary<Guid, DeanUser>(64);

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DEAN_GetUsers]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

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

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            // Users
                            while (reader.Read())
                            {
                                DeanUser user = new DeanUser
                                    {
                                        Id = (Guid)reader["user_id"],
                                        UserName = (string)reader["user_name"],
                                        Name = (string)reader["first_name"] + " " + (string)reader["last_name"],
                                        Levels = new List<int>(1),
                                        ProLevels = new List<Tuple<int, int>>(1),
                                        PrimaryLevel = 0,
                                        PrimaryProLevel = new Tuple<int, int>(0, 0)
                                    };

                                users.Add(user.Id, user);
                            }

                            reader.NextResult();

                            DeanUser lastUser = null;
                            bool lastUserHasGenericPrimary = false;

                            // Levels
                            while (reader.Read())
                            {
                                Guid id = (Guid)reader["user"];

                                if (lastUser == null || lastUser.Id != id)
                                {
                                    users.TryGetValue(id, out lastUser);
                                    lastUserHasGenericPrimary = false;
                                }

                                if (lastUser != null)
                                {
                                    int level = (int)reader["level"];

                                    ((List<int>)lastUser.Levels).Add(level);

                                    if ((bool)reader["primary"])
                                    {
                                        int? languageTo = reader["language_to"] as int?;

                                        if (languageTo.HasValue)
                                        {
                                            lastUserHasGenericPrimary = true;
                                            lastUser.PrimaryLevel = level;

                                        }
                                        else if (!lastUserHasGenericPrimary)
                                        {
                                            lastUser.PrimaryLevel = level;
                                        }
                                    }
                                }
                            }

                            reader.NextResult();

                            // ProLevels
                            while (reader.Read())
                            {
                                Guid id = (Guid)reader["user"];

                                if (lastUser == null || lastUser.Id != id)
                                {
                                    users.TryGetValue(id, out lastUser);
                                    lastUserHasGenericPrimary = false;
                                }

                                if (lastUser != null)
                                {
                                    int proLevel = (int)reader["pro_level"];
                                    int? proSubLevel = reader["pro_sub_level"] as int?;

                                    ((List<Tuple<int, int>>)lastUser.ProLevels).Add(new Tuple<int, int>(proLevel, proSubLevel ?? 0));

                                    if ((bool)reader["primary"])
                                    {
                                        int? languageTo = reader["language_to"] as int?;

                                        if (languageTo.HasValue)
                                        {
                                            lastUserHasGenericPrimary = true;
                                            lastUser.PrimaryProLevel = new Tuple<int, int>(proLevel, proSubLevel ?? 0);
                                        }
                                        else if (!lastUserHasGenericPrimary)
                                        {
                                            lastUser.PrimaryProLevel = new Tuple<int, int>(proLevel, proSubLevel ?? 0);
                                        }
                                    }
                                }
                            }
                        });
            }

            return users.Values.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase).ToArray();
        }

        public static IEnumerable<DeanGroup> GetGroups(Guid userId, Guid appId, Language from, Language to)
        {
            Dictionary<Guid, DeanGroup> groups = new Dictionary<Guid, DeanGroup>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DEAN_GetGroups]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

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

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            // Groups
                            while (reader.Read())
                            {
                                DeanGroup group = new DeanGroup
                                    {
                                        Id = (Guid)reader["group"],
                                        GroupName = (string)reader["name"],
                                        Active = (bool)reader["active"],
                                        Levels = new List<DeanGroupLevel>(1),
                                        PrimaryLevel = 0
                                    };

                                groups.Add(group.Id, group);
                            }

                            reader.NextResult();

                            DeanGroup lastGroup = null;
                            bool lastGroupHasGenericPrimary = false;

                            // Levels
                            while (reader.Read())
                            {
                                Guid id = (Guid)reader["group"];

                                if (lastGroup == null || lastGroup.Id != id)
                                {
                                    groups.TryGetValue(id, out lastGroup);
                                    lastGroupHasGenericPrimary = false;
                                }

                                if (lastGroup != null)
                                {
                                    int level = (int)reader["level"];

                                    ((List<DeanGroupLevel>)lastGroup.Levels).Add(
                                        new DeanGroupLevel
                                            {
                                                Level = level,
                                                Active = (bool)reader["active"],
                                                StartDate = reader["start_date"] as DateTime?,
                                                EndDate = reader["end_date"] as DateTime?
                                            });

                                    if ((bool)reader["primary"])
                                    {
                                        int? languageTo = reader["language_to"] as int?;

                                        if (languageTo.HasValue)
                                        {
                                            lastGroupHasGenericPrimary = true;
                                            lastGroup.PrimaryLevel = level;
                                        }
                                        else if (!lastGroupHasGenericPrimary)
                                        {
                                            lastGroup.PrimaryLevel = level;
                                        }
                                    }
                                }
                            }
                        });
            }

            return groups.Values.OrderBy(x => x.GroupName, StringComparer.InvariantCultureIgnoreCase).ToArray();
        }
    }
}
