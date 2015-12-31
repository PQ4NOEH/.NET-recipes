namespace Heracles.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.WiseNet;

    public abstract partial class WiseNetService : Service<IWiseNetChannel>
    {
        public static int GetArticle(Guid userId, Language language, string uri)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetArticle]"))
            {
                SqlDatabaseManager.AddParameter(command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(command,
                    "@uri",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    uri);

                SqlDatabaseManager.AddParameter(command, "@reference", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                return command.Parameters["@reference"].Value as int? ?? -1;
            }
        }

        public static int CreateArticle(Guid userId, Language language, string uri, int offsetDate)
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
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                   language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(command,
                    "@uri",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    uri);

                SqlDatabaseManager.AddParameter(command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    offsetDate);

                SqlDatabaseManager.AddParameter(command, "@reference", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                return command.Parameters["@reference"].Value as int? ?? -1;
            }
        }
    }
}
