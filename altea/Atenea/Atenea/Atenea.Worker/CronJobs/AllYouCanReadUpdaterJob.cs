namespace Atenea.Worker.CronJobs
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;

    using Atenea.AllYouCanReadUpdater;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    using Quartz;

    public abstract class AllYouCanReadUpdaterJob
    {
        private static readonly int ConnectionRetries;
        private static readonly int ConnectionRetryWaitSeconds;

        static AllYouCanReadUpdaterJob()
        {
            AllYouCanReadUpdaterJob.ConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetries"]);

            AllYouCanReadUpdaterJob.ConnectionRetryWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetryWaitSeconds"]);
        }

        protected static CloudBlobContainer SyncContainer { get; private set; }

        protected AllYouCanReadUpdater Updater { get; private set; }

        protected AllYouCanReadUpdaterJob(AllYouCanReadType type)
        {
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            client.DefaultRequestOptions.RetryPolicy =
                new LinearRetry(
                    TimeSpan.FromSeconds(AllYouCanReadUpdaterJob.ConnectionRetryWaitSeconds),
                    AllYouCanReadUpdaterJob.ConnectionRetries);

            SyncContainer = client.GetContainerReference(ConfigurationManager.AppSettings["AlteaSynchronizer"]);
            SyncContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            CloudBlobContainer container =
                client.GetContainerReference(ConfigurationManager.AppSettings["WiseNetStorageContainer"]);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            this.Updater = new AllYouCanReadUpdater(container, type);
            this.Updater.Message += message =>
                {
                    Trace.TraceWarning("{0}: {1}", RoleEnvironment.CurrentRoleInstance.Id, message);
                };
        }

        public abstract string CronSchedule { get; }
        protected abstract string CloudBlockBlobName { get; }
        protected abstract TimeSpan WaitTime { get; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                this.Run();

                Trace.TraceInformation(
                    "{0}: Finished executing {1} ({2})",
                    RoleEnvironment.CurrentRoleInstance.Id,
                    this.GetType().Name,
                    DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            }
            catch (Exception ex)
            {
                WorkerRole.RaygunTraceException(ex);

                Trace.TraceError(
                    "{0}: Exception in {1} - {2}: {3} ({4})",
                    RoleEnvironment.CurrentRoleInstance.Id,
                    this.GetType().Name,
                    ex.GetType().Name,
                    ex.Message,
                    DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
        }

        public void Run()
        {
#if !DEBUG
            CloudBlockBlob blob = null;
            bool leaseAcquired = false;
            AccessCondition accessCondition = null;

            try
            {
                blob = SyncContainer.GetBlockBlobReference(this.CloudBlockBlobName);
                string leaseId = blob.AcquireLease(null, null);
                accessCondition = new AccessCondition() { LeaseId = leaseId };
                leaseAcquired = true;

                string leaseContent = blob.DownloadText(accessCondition: accessCondition);

                if (string.IsNullOrEmpty(leaseContent)
                    || DateTime.UtcNow.Subtract(DateTime.Parse(leaseContent, CultureInfo.InvariantCulture)) > this.WaitTime)
                {
                    string leaseDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    this.Updater.Run();
                    blob.UploadText(leaseDate, accessCondition: accessCondition);
                }
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode != "LeaseAlreadyPresent")
                {
                    throw;
                }
            }
            finally
            {
                if (leaseAcquired)
                {
                    blob.ReleaseLease(accessCondition);
                }
            }
#endif
        }

        public void Stop()
        {
        }
    }
}
