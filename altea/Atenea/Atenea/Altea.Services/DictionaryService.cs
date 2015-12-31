namespace Altea.Services
{
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Dictionary;

    public class DictionaryService : IService, IDictionaryContract
    {
        public void InsertTextToSpeech(DictionaryInsertSpeechModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[SPEECH_Insert]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Word);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@audio",
                    ParameterDirection.Input,
                    SqlDbType.VarBinary,
                    model.Audio);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.Dictionary);
            }
        }
    }
}
