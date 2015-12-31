namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.Achievements;
    using Altea.Common.Classes;
    using Altea.Database;
    using Altea.Extensions;

    public abstract class AchievementsService
    {
        public static IEnumerable<AchievementCategory> GetCategories()
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[ACHIEVEMENT_GetCategories]"))
            {
            }

            return null;
        }

        public static IEnumerable<Achievement> Get()
        {
            return null;
        }

        public static IEnumerable<UserAchievement> GetUserData(Guid userId, Guid appId, Language from, Language to)
        {
            List<UserAchievement> achievements = new List<UserAchievement>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[ACHIEVEMENT_GetUnlocked]"))
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

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                UserAchievement achievement = new UserAchievement
                                    {
                                        Achievement = (int)reader["achievement"],
                                        Level = (int)reader["level"],
                                        UnlockDate = (DateTime)reader["unlock_date"]
                                    };

                                achievements.Add(achievement);
                            }
                        });
            }

            return achievements;
        }

        public static int GetUserPoints(Guid userId)
        {
            int points;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[ACHIEVEMENTS_GetUserPoints]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(command, "@points", ParameterDirection.ReturnValue, SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                points = (int)command.Parameters["@points"].Value;
            }

            return points;
        } 
        
        public static bool Check(Guid userId, Guid appId, Language from, Language to, string procedure)
        {
            bool status;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, procedure))
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
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (int)command.Parameters["@status"].Value == 0;
            }

            return status;
        }

        public static bool Unlock(Guid userId, Guid appId, Language from, Language to, int achievement, int level)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[ACHIEVEMENTS_Unlock]"))
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
                    "@achievement",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    achievement);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 0;
            }

            return status;
        }
    }
}
