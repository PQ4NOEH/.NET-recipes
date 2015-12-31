namespace Heracles.Worker.Jobs
{
    using System;
    using System.Data;
    using System.Data.Linq;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;

    using Altea.Classes.WiseReader;
    using Altea.Common.Classes;
    using Altea.Database;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage.Queue;

    using Quartz;

    public partial class WiseReaderFileAnalyzerJob : IWorkerJob
    {
        private readonly DataContext dataContext;

        public string CronSchedule
        {
            get
            {
                // Job fires every 15 seconds
                return "0/15 * * * * ? *";
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                this.Run();
            }
            catch (Exception ex)
            {
                WorkerRole.RaygunTraceException(ex);

                Trace.TraceError(
                    "{0}: Exception in {1} - {2} ({3})",
                    RoleEnvironment.CurrentRoleInstance.Id,
                    this.GetType().Name,
                    ex.Message,
                    DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
        }

        public void Run()
        {
            string name = null;
            FileType contentType = FileType.Undefined;
            Language language = Language.NoLanguage;

            CloudQueue queue = null;
            CloudQueueMessage message = null;
            
            try
            {
                queue = StorageService.GetWiseReaderProcessingQueue();
                message = queue.GetMessage(TimeSpan.FromHours(1d));

                if (message == null)
                {
                    return;
                }

                string[] data = message.AsString.Split('|');

                name = data[0];
                if (!Enum.TryParse(data[1], out contentType) || contentType == FileType.Undefined)
                {
                    // Invalid content type, dequeue message.
                    queue.DeleteMessage(message);
                    return;
                }

                if (!Enum.TryParse(data[2], out language) || language == Language.NoLanguage)
                {
                    // Invalid language, dequeue message.
                    queue.DeleteMessage(message);
                    return;
                }

                bool processed, converted, execute;
                bool processException;

                switch (contentType)
                {
                    case FileType.TXT:
                        // Nothing to process
                        processed = true;
                        converted = false;
                        execute = true;
                        processException = false;
                        break;

                    case FileType.PDF:
                        // Find text and OCR scan
                        processed = this.ProcessPdf(name, language, out converted, out execute, out processException);
                        break;

                    //case FileType.DOCX:
                        // DOCX to HTML converter
                        //processed = this.ProcessDocx(name, out converted, out execute, out processException);
                        //break;

                    default:
                        processed = false;
                        converted = false;
                        execute = true;
                        processException = true;
                        break;
                }

                if (execute)
                {
                    if (processed)
                    {
                        this.SetProcessedFile(name, contentType, converted);
                    }
                    else if (processException)
                    {
                        this.SetInvalidFile(name, contentType);
                    }
                }

                queue.DeleteMessage(message);
            }
            catch (Exception ex)
            {
                if (queue != null && message != null && message.DequeueCount < 10)
                {
                    if (message.DequeueCount < 10)
                    {
                        queue.UpdateMessage(message, TimeSpan.FromMinutes(1d), MessageUpdateFields.Visibility);
                    }
                    else
                    {
                        this.SetInvalidFile(name, contentType);
                    }
                }

                if (name != null)
                {
                    Trace.TraceError(
                        "{0}: Exception in {1} - {2} - {3} - {4} - {5} ({6})",
                        RoleEnvironment.CurrentRoleInstance.Id,
                        this.GetType().Name,
                        contentType.ToString(),
                        language.ToString(),
                        name,
                        ex.Message,
                        DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                }

                throw;
            }
        }

        private void SetProcessedFile(string name, FileType type, bool converted)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_SetProcessedFile]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    type.ToString().ToLower());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@converted",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    converted);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        private void SetInvalidFile(string name, FileType type)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISEREADER_SetInvalidFile]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    type.ToString().ToLower());

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void Stop()
        {
            if (this.dataContext != null)
            {
                this.dataContext.Dispose();
            }
        }
    }
}
