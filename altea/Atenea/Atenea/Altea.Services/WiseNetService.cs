namespace Altea.Services
{
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;
    using Altea.Models.WiseNet;

    public class WiseNetService : IService, IWiseNetContract
    {
        #region Landing

        public void CheckUserSearchEngines(UserDataBasicModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_CheckUserSearchEngines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageFrom.GetDatabaseId());

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        #endregion


        #region Articles

        public int CreateArticle(WiseNetCreateModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_CreateArticle]"))
            {
                SqlDatabaseManager.AddParameter(command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(command,
                    "@uri",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Uri);

                SqlDatabaseManager.AddParameter(command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(command, "@reference", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                return command.Parameters["@reference"].Value as int? ?? -1;
            }
        }

        #endregion
    }
}
