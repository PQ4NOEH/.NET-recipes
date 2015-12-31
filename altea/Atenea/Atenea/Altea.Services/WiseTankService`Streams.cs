namespace Altea.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseTank;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.WiseTank;

    /// <summary>
    /// The wise tank service.
    /// </summary>
    public partial class WiseTankService : IService, IWiseTankContract
    {
        public void EditBoxWidth(WiseTankBoxWidthModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditBoxWidth]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
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

        public void EditBoxRefreshRate(WiseTankBoxRefreshRateModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditBoxRefreshRate]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@refresh_rate",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.RefreshRate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public Guid CreateStream(WiseTankCreateModel model)
        {
            Guid stream;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_CreateStream]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

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
                    "@stream_id",
                    ParameterDirection.Output,
                    SqlDbType.UniqueIdentifier);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                WiseTankError status = (WiseTankError)command.Parameters["@status"].Value;
                stream = status == WiseTankError.NoError ? (Guid)command.Parameters["@stream_id"].Value : Guid.Empty;
            }

            return stream;
        }

        public WiseTankError EditStreamName(WiseTankEditStreamModel model)
        {
            WiseTankError status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_EditStreamName]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (WiseTankError)command.Parameters["@status"].Value;
            }

            return status;
        }

        public void RepositionStream(WiseTankRepositionStreamModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_RepositionStream]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@position",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Position);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public WiseTankError DeleteStream(WiseTankStreamModel model)
        {
            WiseTankError status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_DeleteStream]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (WiseTankError)command.Parameters["@status"].Value;
            }

            return status;
        }

        public Guid CreateBox(WiseTankCreateBoxModel model)
        {
            Guid box;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_CreateBox]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.BoxType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@query",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Query);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@box_id",
                    ParameterDirection.Output,
                    SqlDbType.UniqueIdentifier);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                WiseTankError status = (WiseTankError)command.Parameters["@status"].Value;
                box = status == WiseTankError.NoError ? (Guid)command.Parameters["@box_id"].Value : Guid.Empty;
            }

            return box;
        }

        public void RepositionBox(WiseTankRepositionBoxModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_RepositionBox]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@box",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Box);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@position",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Position);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public WiseTankError DeleteBox(WiseTankBoxModel model)
        {
            WiseTankError status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_DeleteBox]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@stream",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Stream);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@box",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Box);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@status",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (WiseTankError)command.Parameters["@status"].Value;
            }

            return status;
        }
    }
}
