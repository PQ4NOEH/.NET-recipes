namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Desks;

    public abstract partial class DesksService : Service<IDesksChannel>
    {
        public static DesksExtraList GetExtraList(Language language, int level, DesksExtraQuestionType type)
        {
            DesksExtraList list = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXTRA_List]"))
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
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    type);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => list = DesksService.GetExtraList(reader));
            }

            return list;
        }

        internal static DesksExtraList GetExtraList(SqlDataReader reader)
        {
            List<DesksExtraArea> areas = new List<DesksExtraArea>();
            List<DesksExtraPart> parts = new List<DesksExtraPart>();

            DesksExtraList list = new DesksExtraList
                {
                    Areas = areas,
                    Parts = parts
                };

            DesksExtraArea lastArea = null;
            while (reader.Read())
            {
                int areaId = (int)reader["id"];

                if (lastArea == null || lastArea.Id != areaId)
                {
                    lastArea = new DesksExtraArea
                        {
                            Id = areaId,
                            Name = (string)reader["name"],
                            Position = (int)reader["position"],
                            ExtraData = reader["extra_data"] as string,
                            SubAreas = new List<DesksExtraArea>(),
                            Types = new List<DesksExtraType>(),
                        };

                    areas.Add(lastArea);
                }

                int? typeId = reader["type_id"] as int?;

                if (typeId.HasValue)
                {
                    DesksExtraType type = new DesksExtraType
                        {
                            Id = typeId.Value,
                            Name = (string)reader["type_name"],
                            Position = (int)reader["type_position"]
                        };

                    ((List<DesksExtraType>)lastArea.Types).Add(type);
                }
            }

            reader.NextResult();

            DesksExtraPart lastPart = null;
            while (reader.Read())
            {
                int id = (int)reader["id"];
                if (lastPart == null || lastPart.Id != id)
                {
                    lastPart = new DesksExtraPart
                        {
                            Id = id,
                            Area = (int)reader["area"],
                            MainData = reader["main_data"] as string
                        };

                    parts.Add(lastPart);
                }

                int? typeId = reader["type"] as int?;
                if (typeId.HasValue)
                {
                    DesksExtraPartType type = new DesksExtraPartType
                        {
                            Type = typeId.Value,
                            MainData = reader["type_main_data"] as string
                        };

                    if (lastPart.Types == null)
                    {
                        lastPart.Types = new List<DesksExtraPartType>();
                    }

                    ((List<DesksExtraPartType>)lastPart.Types).Add(type);
                }
            }

            return list;
        }

        public static long GetExtraAssignmentId(
            Guid userId,
            int level,
            int part,
            bool allowLocal,
            bool allowRemote)
        {
            long id;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXTRA_GetAssignmentId]"))
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
                    "@part",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    part);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowLocal);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowRemote);

                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Output, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                id = (long)command.Parameters["@id"].Value;
            }

            return id;
        }

        public static bool IsExtraAssigned(Guid userId, long id, bool allowLocal, bool allowRemote)
        {
            return DesksService.IsExamAssigned(userId, id, null, allowLocal, allowRemote);
        }

        public static bool IsExtraAssigned(Guid userId, long id, string code, bool allowLocal, bool allowRemote)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_IsAssigned]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowLocal);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowRemote);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        public static DesksExtraPart GetExtraData(long id)
        {
            return null;
        }

        public static IEnumerable<DesksExtraAssignment> GetExtraAssignments(Guid userId, int level)
        {
            List<DesksExtraAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXTRA_GetAssignments]"))
            {
                assignments = GetExtraAssignments(userId, level, command);
            }

            return assignments;
        }

        public static IEnumerable<DesksExtraAssignment> GetExtraGroupAssignments(Guid groupId, int level)
        {
            List<DesksExtraAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXTRA_GetGroupAssignments]"))
            {
                assignments = GetExtraAssignments(groupId, level, command);
            }

            return assignments;
        }

        private static List<DesksExtraAssignment> GetExtraAssignments(Guid uid, int level, SqlCommand command)
        {
            List<DesksExtraAssignment> assignments = new List<DesksExtraAssignment>();

            SqlDatabaseManager.AddParameter(
                command,
                "@uid",
                ParameterDirection.Input,
                SqlDbType.UniqueIdentifier,
                uid);

            SqlDatabaseManager.AddParameter(
                command,
                "@level",
                ParameterDirection.Input,
                SqlDbType.Int,
                level);

            SqlDatabaseManager.ExecuteReader(
                command,
                SqlConnectionString.DataWarehouse,
                reader =>
                    {
                        while (reader.Read())
                        {
                            DesksExtraAssignment assignment = new DesksExtraAssignment
                                {
                                    Id = (int)reader["part"],
                                    Type = reader["type"] as int? ?? 0,
                                    Assigned = (bool)reader["assigned"],
                                    RemoteAssignment = (bool)reader["remote_assignment"],
                                    Blocked = (bool)reader["blocked"],
                                    Finished = (bool)reader["finished"],
                                    Certified = (bool)reader["certified"]
                                };

                            assignments.Add(assignment);
                        }
                    });

            return assignments;
        }

        #region Manage

        public static void ExtraAssign(
            Guid member,
            AlteaMemberType memberType,
            DesksExtraQuestionType assignmentType,
            int level,
            int part,
            int type,
            bool remote,
            bool status,
            bool unblock,
            Guid teacher)
        {
            DesksAssignExtraModel model = new DesksAssignExtraModel
                {
                    Member = member,
                    MemberType = memberType,
                    AssignmentType = assignmentType,
                    Level = level,
                    Part = part,
                    Type = type,
                    Remote = remote,
                    Assign = status,
                    Unblock = unblock,
                    AssignmentTeacher = teacher
                };

            DesksService.Execute("AssignExtra", model);
        }

        #endregion
    }
}
