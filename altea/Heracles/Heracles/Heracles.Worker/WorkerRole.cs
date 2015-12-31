namespace Heracles.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using Mindscape.Raygun4Net;

    using Quartz;
    using Quartz.Impl;

    /// <summary>
    /// The worker role.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        private const string JobSuffix = "_Job";

        private const string TriggerSuffix = "_Trigger";

        private IScheduler scheduler;

        private IEnumerable<IWorkerJob> workers;

        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /// <summary>
        /// The on start.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections.
            // 12 * [Number of logical CPUs]
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            bool result = base.OnStart();

            Trace.TraceInformation(
                "{0}: role is starting ({1})",
                RoleEnvironment.CurrentRoleInstance.Id,
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            this.SetupQuartz();
            this.FindWorkers();

            return result;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if DEBUG
            Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
#else
            RaygunClient client = new RaygunClient("X8g432kXiu6yV46IZHAsIw==");
            client.Send(e.ExceptionObject as Exception);
#endif
        }

        /// <summary>
        /// The run.
        /// </summary>
        public override void Run()
        {
            Trace.TraceInformation(
                "{0}: role is running ({1})",
                RoleEnvironment.CurrentRoleInstance.Id,
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            foreach (IWorkerJob worker in this.workers)
            {
                try
                {
                    this.Schedule(worker);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "{0}: Exception scheduling {1} - {2} ({3})",
                        RoleEnvironment.CurrentRoleInstance.Id,
                        worker.GetType().Name,
                        ex.Message,
                        DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                }
            }

            this.runCompleteEvent.WaitOne();
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        public override void OnStop()
        {
            Trace.TraceInformation(
                "{0}: Heracles.Worker role is stopping",
                RoleEnvironment.CurrentRoleInstance.Id);

            this.scheduler.PauseAll();

            foreach (IWorkerJob worker in this.workers)
            {
                worker.Stop();
            }

            this.scheduler.Standby();
            this.runCompleteEvent.Set();
            base.OnStop();

            Trace.TraceInformation(
                "{0}: Heracles.Worker role has stopped",
                RoleEnvironment.CurrentRoleInstance.Id);
        }



        private void SetupQuartz()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            this.scheduler = schedulerFactory.GetScheduler();
            this.scheduler.Start();
        }

        private void FindWorkers()
        {
            Type jobInterface = typeof(IWorkerJob);
            IEnumerable<Type> workerTypes =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => jobInterface.IsAssignableFrom(x) && x.Name != jobInterface.Name)
                    .ToList();

            this.workers = workerTypes.Select(x => (IWorkerJob)Activator.CreateInstance(x)).ToArray();
        }

        private void Schedule(IWorkerJob worker)
        {
            if (worker == null)
            {
                return;
            }

            Type workerType = worker.GetType();
            string jobKey = string.Concat(workerType.Name, JobSuffix),
                   triggerKey = string.Concat(workerType.Name, TriggerSuffix);


            if (this.scheduler.CheckExists(new TriggerKey(jobKey)))
            {
                return;
            }

            IJobDetail job = JobBuilder.Create(workerType)
                .WithIdentity(jobKey)
                .Build();

            ITrigger trigger =
                TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(job)
                    .StartNow()
                    .WithCronSchedule(worker.CronSchedule)
                    .Build();

            this.scheduler.ScheduleJob(job, trigger);
        }

        internal static void RaygunTraceException(Exception ex)
        {
#if DEBUG
            Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
            Debug.WriteLine(ex.Message);
#else
            RaygunClient client = new RaygunClient("X8g432kXiu6yV46IZHAsIw==");
            client.Send(ex);
#endif
        }
    }
}
