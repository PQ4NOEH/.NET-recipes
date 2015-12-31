namespace Atenea.Worker.WorkerThreads
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Atenea.Worker.ThreadedRole;

    using Microsoft.Azure;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    internal class QueueWorker : WorkerEntryPoint, IWorkerThread
    {
        private static readonly int ConnectionRetries;
        private static readonly int ConnectionRetryMinWaitSeconds;
        private static readonly int ConnectionRetryMaxWaitSeconds;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private QueueClient client;

        static QueueWorker()
        {
            QueueWorker.ConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["ServiceBusConnectionRetries"]);

            QueueWorker.ConnectionRetryMinWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["ServiceBusConnectionRetryMinWaitSeconds"]);

            QueueWorker.ConnectionRetryMaxWaitSeconds =
                Convert.ToInt32(ConfigurationManager.AppSettings["ServiceBusConnectionRetryMaxWaitSeconds"]);
        }

        public override bool OnStart()
        {
            WorkerEntryPoint.TraceInformation("queue worker is starting ({1})", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            string queueName = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.QueueName");

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(new QueueDescription(queueName)
                {
                    MaxSizeInMegabytes = 1024,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(1d),
                    EnableDeadLetteringOnMessageExpiration = true,
                    RequiresDuplicateDetection = false,
                    DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10d),
                    LockDuration = TimeSpan.FromSeconds(30d),
                    MaxDeliveryCount = 10
                });
            }

            this.client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            this.client.RetryPolicy = new RetryExponential(
                TimeSpan.FromSeconds(QueueWorker.ConnectionRetryMinWaitSeconds),
                TimeSpan.FromSeconds(QueueWorker.ConnectionRetryMaxWaitSeconds),
                QueueWorker.ConnectionRetries);

            bool result = base.OnStart();

            WorkerEntryPoint.TraceInformation("queue worker has been started");

            return result;
        }

        public override void Run()
        {
            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            this.client.OnMessage(receivedMessage =>
            {
                WorkerEntryPoint.TraceInformation("queue worker has received a message ({1})", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                if (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        string body = receivedMessage.GetBody<string>();

                        // Process the message
                        Task.Factory.StartNew(
                            state =>
                            {
                                string message = state as string;

                                if (message != null)
                                {
                                    // Trace.WriteLine("Processing Service Bus message: " + message);
                                    Thread.Sleep(5000);
                                }
                            },
                            body,
                            CancellationToken.None,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Default);
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                }
            });

            this.runCompleteEvent.WaitOne();
        }

        public override void OnStop()
        {
            WorkerEntryPoint.TraceInformation("queue worker is stopping");

            this.cancellationTokenSource.Cancel();

            base.OnStop();

            WorkerEntryPoint.TraceInformation("queue worker has stopped");
        }
    }
}
