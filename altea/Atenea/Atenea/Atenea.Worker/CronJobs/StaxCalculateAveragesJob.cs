namespace Atenea.Worker.CronJobs
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;

    using Altea.Classes.Stax;
    using Altea.Services;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    using Quartz;

    public class StaxCalculateAveragesJob : ICronJob
    {
        private static readonly int ConnectionRetries;
        private static readonly int ConnectionRetryWaitSeconds;

        private static readonly TimeSpan MinTimeSpan;

        static StaxCalculateAveragesJob()
        {
            StaxCalculateAveragesJob.ConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetries"]);

            StaxCalculateAveragesJob.ConnectionRetryWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetryWaitSeconds"]);

            // 24 hours +- 0.01 (sampling error)
            StaxCalculateAveragesJob.MinTimeSpan = TimeSpan.FromTicks((long)(TimeSpan.FromHours(24d).Ticks * 0.99));
        }

        private static CloudBlobContainer syncContainer;

        public string CronSchedule
        {
            get
            {
                // Job fires each 24 hours
                return "0 0 12 * * ? *";
            }
        }

        public StaxCalculateAveragesJob()
        {
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            client.DefaultRequestOptions.RetryPolicy =
                new LinearRetry(
                    TimeSpan.FromSeconds(StaxCalculateAveragesJob.ConnectionRetryWaitSeconds),
                    StaxCalculateAveragesJob.ConnectionRetries);

            syncContainer = client.GetContainerReference(ConfigurationManager.AppSettings["AlteaSynchronizer"]);
            syncContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off, null);
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Trace.TraceInformation(
                    "{0}: Executing {1} ({2})",
                    RoleEnvironment.CurrentRoleInstance.Id,
                    this.GetType().Name,
                    DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

                this.Run();
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
                blob = syncContainer.GetBlockBlobReference("stax_averages_lease.lck");
                string leaseId = blob.AcquireLease(null, null);
                accessCondition = new AccessCondition() { LeaseId = leaseId };
                leaseAcquired = true;

                string leaseContent = blob.DownloadText(accessCondition: accessCondition);

                if (string.IsNullOrEmpty(leaseContent)
                    || DateTime.UtcNow.Subtract(DateTime.Parse(leaseContent, CultureInfo.InvariantCulture)) > StaxCalculateAveragesJob.MinTimeSpan)
                {
                    string leaseDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    StaxCalculateAveragesJob.StaxCalculateAverages();
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

        private static void StaxCalculateAverages()
        {
            Array stackTypes = Enum.GetValues(typeof(StackType));

            foreach (StackType type in stackTypes)
            {
                DateTime date =
                    new DateTime(
                        DateTime.UtcNow.Year,
                        DateTime.UtcNow.Month,
                        DateTime.UtcNow.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc).AddDays(-1);

                StaxService.CalculateAverages(type, date);
            }
        }
    }
}
