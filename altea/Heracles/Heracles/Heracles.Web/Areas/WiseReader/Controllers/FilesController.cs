namespace Heracles.Web.Areas.WiseReader.Controllers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using Altea.Classes.WiseReader;

    using Heracles.Models.WiseReader;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    using Microsoft.WindowsAzure.ServiceRuntime;

    [AlteaAuth(Modules = "WiseReader")]
    public class FilesController : AlteaController
    {
        // GET: /WiseReader/TextEditor/{id}
        [HttpGet, AlteaAuth(Modules = "WiseReader:Text Editor")]
        public ActionResult TextEditor(Guid? id)
        {
            WiseReaderTextEditorModel model = new WiseReaderTextEditorModel
                {
                    Id = id
                };

            return this.View("TextEditor", model);
        }

        // POST: /WiseReader/Upload
        [HttpPost]
        public ActionResult Upload(Guid folder)
        {
            // There be dragons!
            LocalResource store = RoleEnvironment.GetLocalResource("WiseBoardStore");
            string userDirectory = Path.Combine(store.RootPath, this.AlteaUser.Id.ToString());
            Directory.CreateDirectory(userDirectory);
            Random random = new Random();

            IDictionary<string, string> savedFiles = new Dictionary<string, string>();
            IDictionary<string, FileType> mimeFiles = new Dictionary<string, FileType>();

            IDictionary<string, FileUploadStatus> uploadStatus = new ConcurrentDictionary<string, FileUploadStatus>();
            IDictionary<string, string> fileIds = new ConcurrentDictionary<string, string>();
            IDictionary<string, string> processedFiles = new ConcurrentDictionary<string, string>();

            foreach (string upload in this.Request.Files)
            {
                HttpPostedFileBase file = this.Request.Files[upload];
                if (file == null || file.ContentLength <= 0)
                {
                    continue;
                }

                string name = file.FileName;

                FileType fileType = WiseReaderService.GetFileMimeType(file.ContentType);

                if (!WiseReaderService.CheckValidFileMimeType(fileType))
                {
                    uploadStatus.Add(name, FileUploadStatus.Invalid);
                    continue;
                }

                // OK, maybe I'm just paranoid about duplicate filenames.
                string fileName;

                using (HashAlgorithm hash = SHA1.Create())
                {
                    fileName = hash.ComputeHash(Encoding.Default.GetBytes(name))
                        .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2")))
                    + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff")
                    + "_" + random.Next();
                }

                string filePath = Path.Combine(userDirectory, fileName);

                file.SaveAs(filePath);
                savedFiles.Add(name, filePath);
                mimeFiles.Add(name, fileType);
            }

            foreach (KeyValuePair<string, string> file in savedFiles)
            {
                FileStream stream = null;

                try
                {
                    stream = System.IO.File.OpenRead(file.Value);
                }
                catch
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }

                    uploadStatus.Add(file.Key, FileUploadStatus.UnknownError);
                    System.IO.File.Delete(file.Value);
                    continue;
                }

                FileType fileType = mimeFiles[file.Key];

                if (!WiseReaderService.CheckFileType(stream, fileType))
                {
                    stream.Dispose();
                    uploadStatus.Add(file.Key, FileUploadStatus.MimeTypeError);
                    System.IO.File.Delete(file.Value);
                    continue;
                }

                string fileNameWithoutExtension = file.Key.Substring(0, file.Key.LastIndexOf('.')).Trim();
                string normalizedFileName = fileNameWithoutExtension.ToLowerInvariant();

                if (normalizedFileName.Length > 260)
                {
                    System.IO.File.Delete(file.Value);
                    uploadStatus.Add(file.Key, FileUploadStatus.UnknownError);
                    continue;
                }

                if (WiseReaderService.CheckFileNameExists(this.AlteaUser.Id, folder, file.Key))
                {
                    System.IO.File.Delete(file.Value);
                    uploadStatus.Add(file.Key, FileUploadStatus.UnknownError);
                    continue;
                }

                byte[] checksum;
                using (HashAlgorithm hash = SHA1.Create())
                {
                    checksum = hash.ComputeHash(stream);
                }

                stream.Dispose();
                string fileId;

                FileUploadStatus status = WiseReaderService.CheckFileStatus(checksum, fileType, out fileId);
                uploadStatus.Add(file.Key, status);

                if (status == FileUploadStatus.UnknownError)
                {
                    // This should not happen, only in case of any database error.
                    System.IO.File.Delete(file.Key);
                    continue;
                }

                if (!status.HasFlag(FileUploadStatus.Exists))
                {
                    string contentType = WiseReaderService.GetFileMimeType(fileType);
                    fileId = WiseReaderService.UploadFileToStorage(file.Value, contentType);
                    System.IO.File.Delete(file.Value);

                    WiseReaderService.QueueProcessFile(
                        fileId,
                        this.AlteaUser.From,
                        checksum,
                        fileType,
                        this.AlteaUser.Id);
                }
                else if (status.HasFlag(FileUploadStatus.Processed))
                {
                    processedFiles.Add(file.Key, fileId);
                }

                WiseReaderService.ReferenceFileAndUser(
                    this.AlteaUser.Id,
                    folder,
                    this.AlteaUser.From,
                    fileId,
                    fileType,
                    fileNameWithoutExtension);

                fileIds.Add(file.Key, fileId);
            }

            WiseReaderUploadModel model = new WiseReaderUploadModel
            {
                FileIds = fileIds,
                FileTypes = mimeFiles,
                ProcessedFiles = processedFiles,
                UploadStatus = uploadStatus,
            };

            return this.JsonNet(model);
        }

        // GET: /WiseReader/View
        [HttpGet]
        public ActionResult View(Guid id)
        {
            BoardStorageFile file = WiseReaderService.GetFileData(this.AlteaUser.Id, id);
            if (file == null)
            {
                return this.RedirectToAction("Index", "Index", new { area = "WiseReader" });
            }

            Folder folder = WiseReaderService.GetFileFolder(id);

            WiseReaderFileViewerModel model = new WiseReaderFileViewerModel
                {
                    FileId = id,
                    FileName = file.Name,
                    Opened = file.Opened,
                    Converted = file.Converted,
                    FolderId = folder.Id,
                    FolderName = folder.Name,
                    FolderLevel = folder.Level,
                    ReferenceId = WiseReaderService.GetReferenceId(id)
                };

            return this.View("View", model);
        }

        // GET: /WiseReader/File
        [HttpGet]
        public ActionResult FileViewer(Guid id)
        {
            BoardStorageFile file = WiseReaderService.GetFileData(this.AlteaUser.Id, id);
            if (file == null)
            {
                return this.HttpNotFound();
            }

            WiseReaderFileModel model = new WiseReaderFileModel
                {
                    FileId = id
                };

            return this.View("FileViewer", model);
        }


        // GET: /WiseReader/Files/File
        [HttpGet]
        public ActionResult File(Guid id)
        {
            BoardStorageFile file = WiseReaderService.GetFileData(this.AlteaUser.Id, id);

            if (file == null)
            {
                return new HttpNotFoundResult();
            }

            Stream stream = null;

            try
            {
                string contentType;
                stream = WiseReaderService.DownloadFileFromStorage(
                    file.StorageName,
                    file.Uploaded,
                    file.Converted,
                    out contentType);

                if (contentType != WiseReaderService.GetFileMimeType(file.Type))
                {
                    return new HttpNotFoundResult();
                }

                stream.Position = 0;
                return this.File(stream, contentType);
            }
            catch
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                return new HttpNotFoundResult();
            }
        }
    }
}