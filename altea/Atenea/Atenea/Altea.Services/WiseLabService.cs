namespace Altea.Services
{
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseLab;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.WiseLab;

    public class WiseLabService : IService, IWiseLabContract
    {
        public WiseLabError SearchWord(WiseLabHuntDataModel model)
        {
            return AddHuntData(model, true);
        }

        public WiseLabError AddHuntData(WiseLabHuntDataModel model)
        {
            return AddHuntData(model, false);
        }

        private static WiseLabError AddHuntData(WiseLabHuntDataModel model, bool searched)
        {
            WiseLabError error;

            string normalizedData = model.Data.Trim();
            string normalizedSentence = model.Sentence == null ? null : model.Sentence.Trim();
            if (normalizedSentence != null && normalizedSentence.Length == 0)
            {
                normalizedSentence = null;
            }

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISELAB_AddHuntData]"))
            {
                AddGlobalParameters(command, model);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@sentence",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedSentence);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@searched",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    searched);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@inbox_overflow",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.InboxOverflow);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@error",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                error = (WiseLabError)command.Parameters["@error"].Value;
            }

            return error;
        }

        public WiseLabError RemoveHuntData(WiseLabHuntDataModel model)
        {
            WiseLabError error;

            string normalizedData = model.Data.Trim();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISELAB_RemoveHuntData]"))
            {
                AddGlobalParameters(command, model);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedData);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@error",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                error = (WiseLabError)command.Parameters["@error"].Value;
            }

            return error;
        }

        public WiseLabError SaveLead(WiseLabWisdomHunterModel model)
        {
            WiseLabError error;

            string normalizedLead = model.Lead.Trim();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISELAB_SaveLead]"))
            {
                AddGlobalParameters(command, model);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@lead",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedLead);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@auto_save",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.AutoSave);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@error",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                error = (WiseLabError)command.Parameters["@error"].Value;
            }

            return error;
        }

        public WiseLabStatus FinishStatus(WiseLabArticleDataModel model)
        {
            WiseLabStatus status;
            WiseLabError error;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISELAB_FinishStatus]"))
            {
                AddGlobalParameters(command, model);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@new_status",
                    ParameterDirection.Output,
                    SqlDbType.Int);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@error",
                    ParameterDirection.ReturnValue,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                status = (WiseLabStatus)command.Parameters["@new_status"].Value;
                error = (WiseLabError)command.Parameters["@error"].Value;
            }

            if (error != WiseLabError.None)
            {
                status = WiseLabStatus.None;
            }

            return status;
        }

        private static void AddGlobalParameters(SqlCommand command, WiseLabArticleDataModel model)
        {
            SqlDatabaseManager.AddParameter(
                command,
                "@uid",
                ParameterDirection.Input,
                SqlDbType.UniqueIdentifier,
                model.UserId);

            SqlDatabaseManager.AddParameter(
                command,
                "@language_from",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.LanguageFrom.GetDatabaseId());

            SqlDatabaseManager.AddParameter(
                command,
                "@language_to",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.LanguageTo.GetDatabaseId());

            SqlDatabaseManager.AddParameter(
                command,
                "@origin",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Origin);

            SqlDatabaseManager.AddParameter(
                command,
                "@reference",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Reference);
        }
    }
}
