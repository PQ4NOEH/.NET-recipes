namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web.Security;

    using Altea.Classes.Admin;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Admin;
    using Altea.Models.User;

    using Heracles.Models;

    public abstract class UserService : Service<IUserChannel>
    {
        public static User GetUser(MembershipUser membershipUser)
        {
            return UserService.GetUser(membershipUser, Guid.Empty, null);
        }

        public static User GetUser(MembershipUser membershipUser, Guid appId)
        {
            return UserService.GetUser(membershipUser, appId, null);
        }

        public static User GetUser(MembershipUser membershipUser, Guid appId, Action<User> updateCacheFunction)
        {
            User user =
                new User(
                    appId == Guid.Empty
                        ? (Func<string, Guid, Language, Language, IDictionary<string, string>>)null
                        : GetUserSettings,
                    appId,
                    updateCacheFunction)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Id = (Guid)membershipUser.ProviderUserKey,
                    Name = membershipUser.UserName,
                    LastActivity = membershipUser.LastActivityDate,
                    Approved = membershipUser.IsApproved
                };

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetUserData]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    membershipUser.ProviderUserKey);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            reader.Read();

                            user.From = ((int)reader["language_from"]).ParseWordLanguageDatabaseId();
                            user.To = ((int)reader["language_to"]).ParseWordLanguageDatabaseId();

                            user.FirstName = (string)reader["first_name"];
                            user.LastName = (string)reader["last_name"];

                            int level = (int)reader["level"];
                            user.Level = level == 0 ? null : AppService.GetLevel(user.From, level);

                            int proLevel = (int)reader["pro_level"];

                            if (proLevel == 0)
                            {
                                user.ProLevel = null;
                            }
                            else
                            {
                                int? proSubLevel = reader["pro_sublevel"] as int?;
                                user.ProLevel = ProDesksService.GetLevel(user.From, proLevel, proSubLevel);
                            }
                        });
            }
            
            return user;
        }

        public static string GetUserName(Guid uid)
        {
            string username;

            const string SqlQuery = "SELECT [UserName] FROM [dbo].[aspnet_Users] WHERE [UserId] = @uid;";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    uid);

                username = SqlDatabaseManager.ExecuteScalar<string>(command, SqlConnectionString.Altea);
            }

            return username;
        }

        public static bool UserHasLevel(Guid appId, string username)
        {
            return UserService.UserHasLevel(appId, username, (Language)0, (Language)0, 0);
        }

        public static bool UserHasLevel(Guid appId, string username, Language from, Language to)
        {
            return UserService.UserHasLevel(appId, username, from, to, 0);
        }

        public static bool UserHasLevel(Guid appId, string username, Language from, Language to, int levelId)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UserHasLevel]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@app",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    levelId);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        public static bool SetUserLevels(Guid appId, Guid userId, Language from, Language to, IEnumerable<AdminMemberLevel> levels)
        {
            AdminSetMemberLevelsModel model = new AdminSetMemberLevelsModel
                {
                    LanguageFrom = from,
                    LanguageTo = to,
                    Member = userId,
                    Application = appId,
                    Levels = levels
                };

            bool status = Execute<bool>("SetUserLevels", model);
            return status;
        }

        public static bool UserHasProLevel(Guid appId, string username)
        {
            return UserService.UserHasProLevel(appId, username, (Language)0, (Language)0, 0, (int?)null);
        }

        public static bool UserHasProLevel(Guid appId, string username, Language from, Language to)
        {
            return UserService.UserHasProLevel(appId, username, from, to, 0, (int?)null);
        }

        public static bool UserHasProLevel(Guid appId, string username, Language from, Language to, int levelId, int? subLevelId)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UserHasProLevel]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@app",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    levelId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sublevel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subLevelId);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        public static IDictionary<string, string> GetUserSettings(string username, Guid appId, Language from, Language to)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_Users_GetSettings]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                settings.Add((string)reader["option"], (string)reader["value"]);
                            }
                        });
            }

            return settings;
        }

        public static bool CheckRemote(string username, Guid appId)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_Users_CheckRemote]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@UserName",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@Application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@ReturnValue",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);

                return (int)command.Parameters["@ReturnValue"].Value == 1;
            }
        }

        public static string CreateUser(
            Guid appId,
            string firstName,
            string lastName,
            string mail,
            string password,
            string role,
            Language from,
            Language to)
        {
            CreateUserModel model = new CreateUserModel
                {
                    AppId = appId,
                    FirstName = firstName,
                    LastName = lastName,
                    Mail = mail,
                    Password = password,
                    Role = role,
                    LanguageFrom = from,
                    LanguageTo = to
                };

            string username = Execute<string>("CreateUser", model);
            return username;
        }

        public static IDictionary<Guid, UserData> GetUsersData(IEnumerable<Guid> userIds)
        {
            IDictionary<Guid, UserData> usersData = new Dictionary<Guid, UserData>();

            using (DataTable usersTable = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetUsersData]"))
            {
                usersTable.Columns.Add("n", typeof(Guid));
                foreach (Guid id in userIds.Distinct())
                {
                    DataRow row = usersTable.NewRow();
                    row["n"] = id;
                    usersTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_ids",
                    ParameterDirection.Input,
                    "[dbo].[guidlist]",
                    usersTable);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                UserData userData = new UserData
                                    {
                                        UserName = (string)reader["UserName"],
                                        FirstName = (string)reader["FirstName"],
                                        LastName = (string)reader["LastName"]
                                    };

                                usersData.Add((Guid)reader["UserId"], userData);
                            }
                        });
            }

            return usersData;
        }
    }
}
