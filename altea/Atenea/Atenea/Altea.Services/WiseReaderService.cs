namespace Altea.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;
    using Altea.Models.WiseReader;

    /// <summary>
    /// The wise reader service.
    /// </summary>
    public class WiseReaderService : IService, IWiseReaderContract
    {
        public void CheckDefaultFolder(UserDataBasicModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_CheckDefaultFolder]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        public Guid CreateFolder(WiseReaderFolderNameModel model)
        {
            Guid? folder;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_CreateFolder]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@parent_folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Folder);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder_id",
                    ParameterDirection.Output,
                    SqlDbType.UniqueIdentifier);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                folder = command.Parameters["@folder_id"].Value as Guid?;
            }

            return folder ?? Guid.Empty;
        }

        public int EditFolder(WiseReaderFolderNameModel model)
        {
            int status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_EditFolder]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Folder);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (int)command.Parameters["@status"].Value;
            }

            return status;
        }

        public int DeleteFolder(WiseReaderFolderModel model)
        {
            int status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_DeleteFolder]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Folder);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (int)command.Parameters["@status"].Value;
            }

            return status;
        }

        public void SaveStorageFile(WiseReaderQueueProcessFileModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_SaveStorageFile]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language == Language.NoLanguage ? (int?)null : model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@checksum",
                    ParameterDirection.Input,
                    SqlDbType.Binary,
                    model.Checksum);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Type.ToString());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@uploader",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Uploader);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        /// <summary>
        /// The reference user and file.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void ReferenceUserAndFile(WiseReaderReferenceFileModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure, 
                    "[dbo].[WISEREADER_ReferenceFileAndUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command, 
                    "@user", 
                    ParameterDirection.Input, 
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.FolderId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language == Language.NoLanguage ? (int?)null : model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@file", 
                    ParameterDirection.Input, 
                    SqlDbType.NVarChar, 
                    model.FileId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.FileType.ToString());

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@name", 
                    ParameterDirection.Input, 
                    SqlDbType.NVarChar, 
                    model.FileName);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }
    }
}
