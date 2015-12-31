namespace Heracles.Worker.Jobs
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    using Altea.Classes.WiseReader;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;

    using Microsoft.WindowsAzure.Storage.Blob;

    public partial class WiseReaderFileAnalyzerJob
    {
        private const string PdfContentType = "application/pdf";

        private bool ProcessPdf(string name, Language language, out bool converted, out bool execute, out bool processException)
        {
            try
            {
                CloudBlobContainer container = StorageService.GetWiseReaderUploadedContainer();
                CloudBlockBlob blob = container.GetBlockBlobReference(name);

                if (!blob.Exists() || blob.Properties.ContentType != PdfContentType)
                {
                    converted = false;
                    execute = true;
                    processException = true;
                    return false;
                }

                bool hasText = false;

                using (Stream stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
                    stream.Position = 0;

                    using (Stream readerStream = new MemoryStream())
                    {
                        stream.CopyTo(readerStream);
                        readerStream.Position = 0;
                        using (PdfReader reader = new PdfReader(readerStream))
                        {
                            int i = 1;
                            while (i <= reader.NumberOfPages)
                            {
                                string pageText = PdfTextExtractor.GetTextFromPage(
                                    reader,
                                    i,
                                    new SimpleTextExtractionStrategy());

                                if (!string.IsNullOrWhiteSpace(pageText))
                                {
                                    hasText = true;
                                    break;
                                }

                                i++;
                            }
                        }
                    }

                    if (hasText)
                    {
                        converted = false;
                        execute = true;
                        processException = false;
                        return true;
                    }

                    // No text found, query ABBYY OCR service
                    stream.Position = 0;
                    this.ProcessPdfOcrAbbyy(stream, language, name);

                    converted = true;
                    execute = false;
                    processException = false;
                    return false;
                }
            }
            catch
            {
                converted = false;
                execute = true;
                processException = true;

                return false;
            }
        }

        private void ProcessPdfOcrAbbyy(Stream stream, Language language, string name)
        {
            HttpWebRequest request = this.CreateOcrAbbyyProcessRequest(language, name);
            this.WriteFileToOcrAbbyyRequest(request, stream);

            string taskId, taskStatus;
            int taskEstimatedProcessingTime;

            // TODO CHECK FAILURE RESPONSE
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            {
                XDocument document = XDocument.Load(new XmlTextReader(responseStream));
                XElement task = document.Root.Element("task");
                taskId = task.Attribute("id").Value;
                taskStatus = task.Attribute("status").Value;
                taskEstimatedProcessingTime = Convert.ToInt32(task.Attribute("estimatedProcessingTime").Value);
            }

            Task.Run(
                () =>
                    {
                        bool exitLoop = false;

                        while (true)
                        {
                            Thread.Sleep(taskEstimatedProcessingTime);

                            HttpWebRequest checkRequest = this.CreateOcrAbbyyCheckRequest(taskId);
                            XElement task;
                            //TODO CHECK FAILURE RESPONSE
                            using (HttpWebResponse response = (HttpWebResponse)checkRequest.GetResponse())
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                XDocument document = XDocument.Load(new XmlTextReader(responseStream));
                                task = document.Root.Element("task");
                                taskId = task.Attribute("id").Value;
                                taskStatus = task.Attribute("status").Value;
                            }

                            switch (taskStatus)
                            {
                                case "Completed":
                                    this.CompletedOcrAbbyy(name, task.Attribute("resultUrl").Value);
                                    exitLoop = true;
                                    break;

                                case "ProcessingFailed":
                                case "Deleted":
                                case "NotEnoughCredits":
                                    this.SetInvalidFile(name, FileType.PDF);
                                    exitLoop = true;
                                    break;

                                default:
                                    taskEstimatedProcessingTime = Convert.ToInt32(task.Attribute("estimatedProcessingTime").Value);
                                    break;

                            }

                            if (exitLoop)
                            {
                                break;
                            }
                        }
                    });
        }

        private void SetRequestDefaultValues(HttpWebRequest request)
        {
            request.UserAgent = ".NET Cloud OCR SDK Client";

            string credentials =
                Convert.ToBase64String(
                    Encoding.GetEncoding("iso-8859-1").GetBytes(string.Concat("AteneaOCR", ":", "HjF2RQTYMOqk96Vvcu6QjLE2")));

            request.Headers.Add(
                "Authorization",
                string.Concat("Basic: ", credentials));
        }

        private HttpWebRequest CreateOcrAbbyyProcessRequest(Language language, string name)
        {
            const string ProcessImageUri =
                "https://cloud.ocrsdk.com/processImage?language={0}" +
                                                     "&profile=documentConversion" +
                                                     "&textType=normal" +
                                                     "&correctOrientation=true" +
                                                     "&exportFormat=pdfSearchable" +
                                                       "&description={1}";

            string uri = string.Format(
                CultureInfo.InvariantCulture,
                ProcessImageUri,
                Uri.EscapeDataString(language.GetPrefix(LanguagePrefixType.OcrAbbyy)),
                Uri.EscapeDataString(name));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            this.SetRequestDefaultValues(request);
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            return request;
        }

        private void WriteFileToOcrAbbyyRequest(HttpWebRequest request, Stream stream)
        {
            const int BufferLength = 1024 * 1024; 
               
            request.ContentLength = stream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] buffer = new byte[BufferLength]; // Read in chunks of 1 MB marx
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, BufferLength);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    requestStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        private HttpWebRequest CreateOcrAbbyyCheckRequest(string taskId)
        {
            const string CheckTaskUri = "https://cloud.ocrsdk.com/getTaskStatus?taskId={0}";

            string uri = string.Format(CultureInfo.InvariantCulture, CheckTaskUri, taskId);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            this.SetRequestDefaultValues(request);
            request.Method = "GET";

            return request;
        }

        private HttpWebRequest CreateOcrAbbyyGetFileRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = ".NET Cloud OCR SDK Client";
            request.Method = "GET";

            return request;
        }

        private void CompletedOcrAbbyy(string name, string uri)
        {
            HttpWebRequest request = this.CreateOcrAbbyyGetFileRequest(uri);
            // TODO CHECK RESPONSE FAILURE
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                CloudBlobContainer container = StorageService.GetWiseReaderProcessedContainer();
                CloudBlockBlob blob = container.GetBlockBlobReference(name);
                blob.Properties.ContentType = "application/pdf";
                blob.UploadFromStream(stream);
                blob.FetchAttributes();
            }

            this.SetProcessedFile(name, FileType.PDF, true);
        }
    }
}
