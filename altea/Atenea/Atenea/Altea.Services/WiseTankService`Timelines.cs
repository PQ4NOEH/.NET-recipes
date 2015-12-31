namespace Altea.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseTank;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;
    using Altea.Models.WiseTank;

    /// <summary>
    /// The wise tank service.
    /// </summary>
    public partial class WiseTankService : IService, IWiseTankContract
    {
        public void CheckDefaultTimelines(UserDataBasicModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_CheckDefaultTimelines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void EditGroupBoxWidth(WiseTankBoxWidthModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditGroupBoxWidth]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Data);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@width",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Width);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public Guid CreateTimeline(WiseTankCreateTimelineModel model)
        {
            Guid timelineId;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_CreateTimeline]"))
            using (DataTable permissionTypesTable = new DataTable())
            {
                permissionTypesTable.Columns.Add("n", typeof(int));
                permissionTypesTable.Columns.Add("m", typeof(int));

                foreach (KeyValuePair<int, int> kvp in model.PermissionTypes)
                {
                    DataRow row = permissionTypesTable.NewRow();
                    row["n"] = kvp.Key;
                    row["m"] = kvp.Value;

                    permissionTypesTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@description",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@access_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AccessType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_comments",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedComments);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@write_own_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.WriteOwnArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@permission_types",
                    ParameterDirection.Input,
                    "[dbo].[intmap]",
                    permissionTypesTable);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@area",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Area);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline_id",
                    ParameterDirection.Output,
                    SqlDbType.UniqueIdentifier);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                if ((int)command.Parameters["@status"].Value == 0)
                {
                    timelineId = (Guid)command.Parameters["@timeline_id"].Value;
                }
                else
                {
                    timelineId = Guid.Empty;
                }
            }

            return timelineId;
        }

        public WiseTankError EditTimeline(WiseTankEditTimelineModel model)
        {
            WiseTankError error;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditTimeline]"))
            using (DataTable permissionTypesTable = new DataTable())
            {
                permissionTypesTable.Columns.Add("n", typeof(int));
                permissionTypesTable.Columns.Add("m", typeof(int));

                foreach (KeyValuePair<int, int> kvp in model.PermissionTypes)
                {
                    DataRow row = permissionTypesTable.NewRow();
                    row["n"] = kvp.Key;
                    row["m"] = kvp.Value;

                    permissionTypesTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@description",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@access_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AccessType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_comments",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedComments);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@write_own_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.WriteOwnArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@permission_types",
                    ParameterDirection.Input,
                    "[dbo].[intmap]",
                    permissionTypesTable);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                error = (int)command.Parameters["@status"].Value == 0 ? WiseTankError.NoError : WiseTankError.UnknownError;
            }

            return error;
        }

        public void SetTimelineArea(WiseTankSetTimelineAreaModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_SetTimelineArea]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@area",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Area);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public Guid AddTimelineColumn(WiseTankAddTimelineColumnModel model)
        {
            Guid columnId;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_AddTimelineColumn]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@ttlevel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.ThinkTankLevel);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@access_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AccessType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@moderated_comments",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.ModeratedComments);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@write_own_articles",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.WriteOwnArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@column_id",
                    ParameterDirection.Output,
                    SqlDbType.UniqueIdentifier);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                if ((int)command.Parameters["@status"].Value == 0)
                {
                    columnId = (Guid)command.Parameters["@column_id"].Value;
                }
                else
                {
                    columnId = Guid.Empty;
                }
            }

            return columnId;
        }

        public WiseTankError AddTimelineUser(WiseTankAddTimelineUserModel model)
        {
            WiseTankError error;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_AddTimelineUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@new_user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.NewUser);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@ttlevel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.ThinkTankLevel);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@role",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Role);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@permissions",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Permissions);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                error = (WiseTankError)((int)command.Parameters["@status"].Value);
            }

            return error;
        }

        public WiseTankError EditTimelineUser(WiseTankAddTimelineUserModel model)
        {
            WiseTankError error;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditTimelineUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@edit_user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.NewUser);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@ttlevel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.ThinkTankLevel);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@role",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Role);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@permissions",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Permissions);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                error = (WiseTankError)((int)command.Parameters["@status"].Value);
            }

            return error;
        }
    }
}
