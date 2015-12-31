namespace Heracles.Worker
{
    using System;
    using System.Configuration;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    internal static class StorageService
    {
        private static readonly int ConnectionRetries;

        private static readonly int ConnectionRetryWaitSeconds;

        private static CloudBlobContainer wiseReaderUploadedContainer;

        private static CloudBlobContainer wiseReaderProcessedContainer;

        private static CloudQueue wiseReaderProcessingQueue;

        static StorageService()
        {
            StorageService.ConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetries"]);

            StorageService.ConnectionRetryWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["StorageConnectionRetryWaitSeconds"]);
        }

        public static CloudBlobContainer GetWiseReaderUploadedContainer()
        {
            if (wiseReaderUploadedContainer == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient client = account.CreateCloudBlobClient();
                client.DefaultRequestOptions.RetryPolicy =
                    new LinearRetry(
                        TimeSpan.FromSeconds(StorageService.ConnectionRetryWaitSeconds),
                        StorageService.ConnectionRetries);
                wiseReaderUploadedContainer = client.GetContainerReference("wisereaderuploaded");
                wiseReaderUploadedContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off);
            }

            return wiseReaderUploadedContainer;
        }

        public static CloudBlobContainer GetWiseReaderProcessedContainer()
        {
            if (wiseReaderProcessedContainer == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient client = account.CreateCloudBlobClient();
                client.DefaultRequestOptions.RetryPolicy =
                    new LinearRetry(
                        TimeSpan.FromSeconds(StorageService.ConnectionRetryWaitSeconds),
                        StorageService.ConnectionRetries);
                wiseReaderProcessedContainer = client.GetContainerReference("wisereaderprocessed");
                wiseReaderProcessedContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off);
            }

            return wiseReaderProcessedContainer;
        }

        public static CloudQueue GetWiseReaderProcessingQueue()
        {
            if (wiseReaderProcessingQueue == null)
            {
                string appName = CloudConfigurationManager.GetSetting("Altea.ApplicationName").ToLowerInvariant().Replace(' ', '-');

                string connectionString = ConfigurationManager.ConnectionStrings["AlteaData"].ConnectionString;
                CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                CloudQueueClient client = account.CreateCloudQueueClient();
                client.DefaultRequestOptions.RetryPolicy =
                    new LinearRetry(
                        TimeSpan.FromSeconds(StorageService.ConnectionRetryWaitSeconds),
                        StorageService.ConnectionRetries);
                wiseReaderProcessingQueue = client.GetQueueReference(appName + "wisereader");
                wiseReaderProcessingQueue.CreateIfNotExists();
            }

            return wiseReaderProcessingQueue;
        }
    }
}
