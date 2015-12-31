namespace Heracles.Worker.Jobs
{
    using System.IO;

    using Microsoft.WindowsAzure.Storage.Blob;

    public partial class WiseReaderFileAnalyzerJob
    {
        private const string DocxContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        private bool ProcessDocx(string name, out bool converted, out bool execute, out bool processException)
        {
            CloudBlobContainer container = StorageService.GetWiseReaderUploadedContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(name);

            if (!blob.Exists() || blob.Properties.ContentType != DocxContentType)
            {
                converted = false;
                execute = true;
                processException = true;
                return false;
            }

            using (Stream stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                stream.Position = 0;
            }

            converted = false;
            execute = false;
            processException = false;
            return false;
        }
    }
}
