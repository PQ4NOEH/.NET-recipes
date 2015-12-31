namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;

    using Altea.Classes.WiseReader;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;
    using Altea.Models.WiseReader;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    public abstract class WiseReaderService : Service<IWiseReaderChannel>
    {
        private static readonly int WiseReaderConnectionRetries;

        private static readonly int WiseReaderConnectionRetryWaitSeconds;

        static WiseReaderService()
        {
            WiseReaderService.WiseReaderConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["WiseReaderConnectionRetries"]);

            WiseReaderService.WiseReaderConnectionRetryWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["WiseReaderConnectionRetryWaitSeconds"]);
        }

        private static class WiseReaderContainers
        {
            public enum ContainerType
            {
                CreatedFiles,
                UploadedFiles,
                ProcessedFiles
            }

            private static CloudBlobContainer createdContainer;

            private static CloudBlobContainer uploadedContainer;

            private static CloudBlobContainer processedContainer;

            private static CloudQueue processingQueue;

            public static CloudBlobContainer GetContainer(ContainerType type)
            {
                string containerName;
                switch (type)
                {
                    case ContainerType.CreatedFiles:
                        if (createdContainer != null)
                        {
                            return createdContainer;
                        }

                        containerName = "wisereadercreated";
                        break;

                    case ContainerType.UploadedFiles:
                        if (uploadedContainer != null)
                        {
                            return uploadedContainer;
                        }

                        containerName = "wisereaderuploaded";
                        break;

                    case ContainerType.ProcessedFiles:
                        if (processedContainer != null)
                        {
                            return processedContainer;
                        }

                        containerName = "wisereaderprocessed";
                        break;

                    default:
                        throw new ArgumentException();
                }

                string connectionString = ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient client = account.CreateCloudBlobClient();
                client.DefaultRequestOptions.RetryPolicy =
                    new LinearRetry(
                        TimeSpan.FromSeconds(WiseReaderService.WiseReaderConnectionRetryWaitSeconds),
                        WiseReaderService.WiseReaderConnectionRetries);
                CloudBlobContainer container = client.GetContainerReference(containerName);
                container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

                switch (type)
                {
                    case ContainerType.CreatedFiles:
                        createdContainer = container;
                        break;

                    case ContainerType.UploadedFiles:
                        uploadedContainer = container;
                        break;

                    case ContainerType.ProcessedFiles:
                        processedContainer = container;
                        break;

                    default:
                        throw new ArgumentException();
                }

                return container;
            }

            public static CloudQueue GetQueue()
            {
                if (processingQueue == null)
                {
                    string appName = CloudConfigurationManager.GetSetting("Altea.ApplicationName").ToLowerInvariant();

                    string connectionString = ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString;
                    CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                    CloudQueueClient client = account.CreateCloudQueueClient();
                    client.DefaultRequestOptions.RetryPolicy =
                        new LinearRetry(
                            TimeSpan.FromSeconds(WiseReaderService.WiseReaderConnectionRetryWaitSeconds),
                            WiseReaderService.WiseReaderConnectionRetries);
                    processingQueue = client.GetQueueReference(appName + "wisereader");
                    processingQueue.CreateIfNotExists();
                }

                return processingQueue;
            }
        }

        public static void CheckDefaultFolder(Guid userId)
        {
            UserDataBasicModel model = new UserDataBasicModel
                {
                    UserId = userId
                };

            WiseReaderService.Execute("CheckDefaultFolder", model);
        }

        public static IDictionary<Guid, Folder> GetFolders(Guid userId)
        {
            IDictionary<Guid, Folder> folders = new Dictionary<Guid, Folder>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_GetFolders]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                Guid folderId = (Guid)reader["id"];
                                Guid? parentId = reader["parent"] as Guid?;

                                Folder folder = new Folder
                                    {
                                        Id = folderId,
                                        Name = (string)reader["name"],
                                        Parent = parentId,
                                        Position = (int)reader["position"]
                                    };

                                folders.Add(folderId, folder);
                            }
                        });
            }

            return folders;
        }

        public static Guid CreateFolder(Guid userId, Guid parentFolder, string name)
        {
            WiseReaderFolderNameModel model = new WiseReaderFolderNameModel
                {
                    UserId = userId,
                    Folder = parentFolder,
                    Name = name
                };

            Guid status = WiseReaderService.Execute<Guid>("CreateFolder", model);
            return status;
        }

        public static int EditFolder(Guid userId, Guid folder, string name)
        {
            WiseReaderFolderNameModel model = new WiseReaderFolderNameModel
                {
                    UserId = userId,
                    Folder = folder,
                    Name = name
                };

            int status = WiseReaderService.Execute<int>("EditFolder", model);
            return status;
        }

        public static int DeleteFolder(Guid userId, Guid folder)
        {
            WiseReaderFolderModel model = new WiseReaderFolderModel
                {
                    UserId = userId,
                    Folder = folder
                };

            int status = WiseReaderService.Execute<int>("DeleteFolder", model);
            return status;
        }

        public static IEnumerable<BoardFile> GetFolderFiles(Guid userId, Guid folderId)
        {
            List<BoardFile> files = new List<BoardFile>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_GetFolderFiles]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    folderId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                FileType type;
                                if (!Enum.TryParse((string)reader["type"], false, out type))
                                {
                                    type = FileType.Undefined;
                                }

                                BoardFile file = new BoardFile
                                    {
                                        Id = (Guid)reader["id"],
                                        Type = type,
                                        Uploaded = (bool)reader["uploaded"],
                                        Language = ((int)reader["language"]).ParseWordLanguageDatabaseId(),
                                        Name = (string)reader["name"],
                                        CreateDate = (DateTime)reader["create_date"],
                                        LastModifiedDate = (DateTime)reader["last_modified_date"],
                                        Processed = (bool)reader["processed"],
                                        Converted = (bool)reader["converted"],
                                        Opened = (bool)reader["opened"],
                                        Invalid = (bool)reader["invalid"]
                                    };

                                files.Add(file);
                            }
                        });
            }

            return files;
        }

        public static BoardStorageFile GetFileData(Guid userId, Guid fileId)
        {
            bool exists = false;
            string name = string.Empty;
            string storageName = string.Empty;
            FileType type = FileType.Undefined;
            bool valid = false;
            bool uploaded = false;
            bool converted = false;
            bool opened = false;

            const string SqlQuery =
                "SELECT [A].[name], [B].[name] AS [storage_name], [B].[file_type], [B].[processed], " +
                "[B].[invalid], [B].[uploaded], [B].[converted], [A].[opened] "
                + "FROM [dbo].[WISEREADER_Files] AS [A] "
                + "INNER JOIN [dbo].[WISEREADER_StorageFiles] AS [B] ON [B].[id] = [A].[file] "
                + "WHERE [A].[user] = @uid AND [A].[id] = @fid;";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@fid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    fileId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                return;
                            }

                            exists = true;
                            name = (string)reader["name"];
                            storageName = (string)reader["storage_name"];
                            type = (FileType)((int)reader["file_type"]);
                            valid = (bool)reader["processed"] && !(bool)reader["invalid"];
                            uploaded = (bool)reader["uploaded"];
                            converted = (bool)reader["converted"];
                            opened = (bool)reader["opened"];
                        });
            }

            if (!exists || !valid)
            {
                return null;
            }

            return new BoardStorageFile
                {
                    Name = name,
                    StorageName = storageName,
                    Type = type,
                    Uploaded = uploaded,
                    Converted = converted,
                    Opened = opened
                };
        }

        public static int GetReferenceId(Guid fileId)
        {
            int referenceId;

            const string SqlQuery = "SELECT [id] FROM [dbo].[WISEREADER_Articles] WHERE [file] = @file;";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@file",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    fileId);

                referenceId = SqlDatabaseManager.ExecuteScalar<int>(command, SqlConnectionString.DataWarehouse);
            }

            return referenceId;
        }

        public static Folder GetFileFolder(Guid fileId)
        {
            Guid folderId = Guid.Empty;
            string folderName = string.Empty;
            int folderLevel = -1;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[WISEREADER_GetFileFolder]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@fid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    fileId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                return;
                            }

                            folderId = (Guid)reader["id"];
                            folderName = (string)reader["name"];
                            folderLevel = (int)reader["level"];
                        });
            }

            return new Folder
                {
                    Id = folderId,
                    Name = folderName,
                    Level = folderLevel
                };
        }

        public static FileType GetFileMimeType(string contentType)
        {
            // I'm paranoid about being paranoid.
            switch (contentType.ToLowerInvariant())
            {
                // TXT files
                case "text/plain":
                case "application/txt":
                case "browser/internal":
                case "text/anytext":
                case "widetext/plain":
                case "widetext/paragraph":
                    return FileType.TXT;

                // PDF files
                case "application/pdf":
                case "application/x-pdf":
                case "application/acrobat":
                case "applications/vnd.pdf":
                case "text/pdf":
                case "text/x-pdf":
                    return FileType.PDF;

                // RTF files
                case "application/rtf":
                case "application/x-rtf":
                case "text/rtf":
                case "text/richtext":
                case "application/x-soffice":
                    return FileType.RTF;

                // ODT files
                case "application/vnd.oasis.opendocument.text":
                case "application/x-vnd.oasis.opendocument.text":
                    return FileType.ODT;

                // DOC files
                case "application/msword":
                case "application/doc":
                case "appl/text":
                case "application/vnd.msword":
                case "application/vnd.ms-word":
                case "application/winword":
                case "application/word":
                case "application/x-msw6":
                case "application/x-msword":
                    return FileType.DOC;

                // DOCX files (WTF is wrong with Microsoft and this huge mime type?)
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    return FileType.DOCX;

                default:
                    return FileType.Undefined;
            }
        }

        public static string GetFileMimeType(FileType contentType)
        {
            switch (contentType)
            {
                case FileType.TXT:
                    return "text/plain";

                case FileType.PDF:
                    return "application/pdf";

                case FileType.RTF:
                    return "application/rtf";

                case FileType.ODT:
                    return "application/vnd.oasis.opendocument.text";

                case FileType.DOC:
                    return "application/doc";

                case FileType.DOCX:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case FileType.Undefined:
                default:
                    return null;
            }
        }

        public static bool CheckValidFileMimeType(FileType type)
        {
            switch (type)
            {
                case FileType.TXT:
                case FileType.PDF:
                    return true;
                
                case FileType.RTF:
                case FileType.ODT:
                case FileType.DOC:
                case FileType.DOCX:
                    return false;
                
                case FileType.Undefined:
                default:
                    return false;
            }
        }

        public static bool CheckFileType(FileStream stream, FileType type)
        {
            int signatures;
            int[] signatureLength;
            byte[][] signatureBytes;
            
            switch (type)
            {
                case FileType.TXT:
                    // TXT files contain only plain text without any signature
                    return true;

                case FileType.PDF:
                    signatures = 1;
                    signatureLength = new int[] { 4 };
                    signatureBytes = new byte[][] { new byte[] { 0x25, 0x50, 0x44, 0x46 } };
                    break;

                case FileType.RTF:
                    signatures = 1;
                    signatureLength = new int[] { 6 };
                    signatureBytes = new byte[][] { new byte[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 } };
                    break;

                case FileType.ODT:
                    signatures = 1;
                    signatureLength = new int[] { 4 };
                    signatureBytes = new byte[][] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } };
                    break;

                case FileType.DOC:
                    // F# you Micro$oft!
                    signatures = 4;
                    signatureLength = new int[] { 8, 4, 8, 4 };
                    signatureBytes = new byte[][]
                                         {
                                             new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                                             new byte[] { 0x0D, 0x44, 0x4F, 0x43 },
                                             new byte[] { 0xCF, 0x11, 0xe0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00 },
                                             new byte[] { 0xEc, 0xA5, 0xC1, 0x00 } 
                                         };
                    break;

                case FileType.DOCX:
                    signatures = 1;
                    signatureLength = new int[] { 8 };
                    signatureBytes = new byte[][] { new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } };
                    break;

                case FileType.Undefined:
                default:
                    return false;
            }


            for (int i = 0; i < signatures; i++)
            {
                bool valid = true;

                for (int j = 0; j < signatureLength[i]; j++)
                {
                    if (stream.ReadByte() != signatureBytes[i][j])
                    {
                        valid = false;
                        break;
                    }
                }

                stream.Position = 0;

                if (valid)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckFileNameExists(Guid userId, Guid folder, string file)
        {
            bool exists;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_CheckFileNameExists]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@folder",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    folder);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@file",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    file);

                exists = SqlDatabaseManager.ExecuteScalar<bool>(command, SqlConnectionString.DataWarehouse);
            }

            return exists;
        }

        public static FileUploadStatus CheckFileStatus(byte[] checksum, FileType type, out string fileId)
        {
            FileUploadStatus status = FileUploadStatus.UnknownError;
            string id = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_CheckFileStatus]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@checksum",
                    ParameterDirection.Input,
                    SqlDbType.Binary,
                    checksum);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@file_type",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    type.ToString().ToLower());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                return;
                            }

                            status = FileUploadStatus.Valid;

                            if ((bool)reader["exists"])
                            {
                                status &= FileUploadStatus.Exists;
                                id = (string)reader["name"];
                            }

                            if ((bool)reader["processed"])
                            {
                                status &= FileUploadStatus.Processed;
                            }

                            if ((bool)reader["converted"])
                            {
                                status &= FileUploadStatus.Converted;
                            }

                            if ((bool)reader["invalid"])
                            {
                                status &= FileUploadStatus.CantProcessError;
                            }
                        });
            }

            fileId = id;
            return status;
        }

        public static string UploadFileToStorage(string filePath, string contentType)
        {
            CloudBlobContainer container =
                WiseReaderContainers.GetContainer(WiseReaderContainers.ContainerType.UploadedFiles);

            string id = Guid.NewGuid().ToString("N");
            using (FileStream stream = File.OpenRead(filePath))
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(id);
                blob.Properties.ContentType = contentType;
                blob.UploadFromStream(stream);
                blob.FetchAttributes();
            }

            return id;
        }

        public static Stream DownloadFileFromStorage(string id, bool uploaded, bool converted, out string contentType)
        {
            WiseReaderContainers.ContainerType containerType;

            if (uploaded)
            {
                containerType = converted
                                    ? WiseReaderContainers.ContainerType.ProcessedFiles
                                    : WiseReaderContainers.ContainerType.UploadedFiles;
            }
            else
            {
                containerType = WiseReaderContainers.ContainerType.CreatedFiles;
            }

            CloudBlobContainer container = WiseReaderContainers.GetContainer(containerType);
            CloudBlockBlob blob = container.GetBlockBlobReference(id);
            blob.FetchAttributes();
            contentType = blob.Properties.ContentType;

            Stream stream = new MemoryStream();
            blob.DownloadToStream(stream);
            return stream;
        }

        public static void QueueProcessFile(
            string id,
            Language language,
            byte[] checksum,
            FileType contentType,
            Guid userId)
        {
            WiseReaderQueueProcessFileModel model = new WiseReaderQueueProcessFileModel
                {
                    Name = id,
                    Language = language,
                    Checksum = checksum,
                    Type = contentType,
                    Uploader = userId
                };

            WiseReaderService.Execute("SaveStorageFile", model);

            CloudQueue queue = WiseReaderContainers.GetQueue();
            CloudQueueMessage message =
                new CloudQueueMessage(string.Concat(id, "|", contentType.ToString(), "|", language.ToString()));
            queue.AddMessage(message);
        }

        public static void ReferenceFileAndUser(
            Guid userId,
            Guid folderId,
            Language language,
            string fileId,
            FileType fileType,
            string fileName)
        {
            WiseReaderReferenceFileModel model = new WiseReaderReferenceFileModel
                {
                    UserId = userId,
                    FolderId = folderId,
                    Language = language,
                    FileId = fileId,
                    FileType = fileType,
                    FileName = fileName
                };

            WiseReaderService.Execute("ReferenceUserAndFile", model);
        }
    }
}
